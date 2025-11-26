using System;
using System.Linq;
using System.Collections.Generic;

public class Tarjeta
{
    protected decimal saldo;
    protected decimal saldoPendiente;
    private const decimal LIMITE_SALDO = 56000m;
    private const decimal SALDO_NEGATIVO_PERMITIDO = 1200m;
    private const decimal TARIFA_BASICA = 1580m;
    protected DateTime ultimoViaje;
    protected int viajesHoy;
    protected DateTime fechaUltimoDia;
    private static int contadorId = 0;
    private int id;
    
    // Nuevos campos para Iteración 4
    protected int viajesMensuales;
    protected DateTime fechaUltimoMes;
    protected DateTime ultimoBoletoParaTrasbordo;
    protected string ultimaLineaUsada;
    protected List<string> lineasUsadasEnTrasbordo;

    // Proveedor de tiempo inyectable
    protected ITiempoProvider tiempoProvider;

    public decimal Saldo
    {
        get { return saldo; }
    }

    public decimal SaldoPendiente
    {
        get { return saldoPendiente; }
    }

    public int Id
    {
        get { return id; }
    }

    public int ViajesMensuales
    {
        get { return viajesMensuales; }
    }

    public Tarjeta() : this(new TiempoReal())
    {
    }

    public Tarjeta(ITiempoProvider tiempoProvider)
    {
        this.tiempoProvider = tiempoProvider;
        saldo = 0m;
        saldoPendiente = 0m;
        ultimoViaje = DateTime.MinValue;
        viajesHoy = 0;
        fechaUltimoDia = DateTime.MinValue;
        id = ++contadorId;
        
        // Inicializar campos para Iteración 4
        viajesMensuales = 0;
        fechaUltimoMes = DateTime.MinValue;
        ultimoBoletoParaTrasbordo = DateTime.MinValue;
        ultimaLineaUsada = "";
        lineasUsadasEnTrasbordo = new List<string>();
    }

    public virtual bool Cargar(decimal monto)
    {
        decimal[] montosAceptados = { 2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000 };

        if (!montosAceptados.Contains(monto))
            return false;

        // Si hay saldo negativo, primero se descuenta de la carga
        if (saldo < 0)
        {
            decimal montoRestante = monto + saldo; // saldo es negativo, entonces suma es resta
            if (montoRestante <= 0)
            {
                saldo += monto;
                return true;
            }
            else
            {
                saldo = 0;
                monto = montoRestante;
            }
        }

        decimal espacioDisponible = LIMITE_SALDO - saldo;

        if (espacioDisponible >= monto)
        {
            saldo += monto;
        }
        else
        {
            saldo = LIMITE_SALDO;
            saldoPendiente += (monto - espacioDisponible);
        }

        return true;
    }

    public void AcreditarCarga()
    {
        if (saldoPendiente <= 0)
            return;

        decimal espacioDisponible = LIMITE_SALDO - saldo;
        
        if (espacioDisponible >= saldoPendiente)
        {
            saldo += saldoPendiente;
            saldoPendiente = 0m;
        }
        else
        {
            saldo = LIMITE_SALDO;
            saldoPendiente -= espacioDisponible;
        }
    }

    public virtual bool Descontar(decimal monto)
    {
        // Permitir saldo negativo hasta el límite permitido
        if (saldo - monto < -SALDO_NEGATIVO_PERMITIDO)
            return false;

        saldo -= monto;
        AcreditarCarga();
        return true;
    }

    public virtual bool PagarPasaje(string lineaColectivo = "", bool esInterurbano = false)
    {
        ActualizarContadorViajesMensuales();
        
        // Incrementar ANTES de calcular la tarifa para que el descuento se aplique correctamente
        viajesMensuales++;
        
        decimal tarifaAPagar = ObtenerTarifa(esInterurbano);
        
        bool resultado = Descontar(tarifaAPagar);
        if (resultado)
        {
            DateTime ahora = tiempoProvider.Ahora;
            ultimoViaje = ahora;
            ultimoBoletoParaTrasbordo = ahora;
            ultimaLineaUsada = lineaColectivo;
            
            // Reiniciar la cadena de trasbordos al pagar un viaje completo
            lineasUsadasEnTrasbordo.Clear();
            lineasUsadasEnTrasbordo.Add(lineaColectivo);
        }
        else
        {
            // Si falla el pago, revertir el incremento
            viajesMensuales--;
        }
        return resultado;
    }

    public virtual decimal ObtenerTarifa(bool esInterurbano = false)
    {
        ActualizarContadorViajesMensuales();
        
        if (esInterurbano)
        {
            return 3000m;
        }
        
        // Boleto de uso frecuente (solo para tarjetas normales)
        if (this.GetType() == typeof(Tarjeta))
        {
            if (viajesMensuales >= 29 && viajesMensuales < 59)
            {
                return TARIFA_BASICA * 0.80m; // 20% de descuento
            }
            else if (viajesMensuales >= 59 && viajesMensuales <= 79)
            {
                return TARIFA_BASICA * 0.75m; // 25% de descuento
            }
        }
        
        return TARIFA_BASICA;
    }

    public virtual string ObtenerTipo()
    {
        return "Normal";
    }

    protected void ActualizarContadorViajes()
    {
        DateTime ahora = tiempoProvider.Ahora;

        if (fechaUltimoDia.Date != ahora.Date)
        {
            viajesHoy = 0;
            fechaUltimoDia = ahora;
        }
        
    }

    protected void ActualizarContadorViajesMensuales()
    {
        DateTime ahora = tiempoProvider.Ahora;
        // Reiniciar contador si cambió el mes
        if (fechaUltimoMes == DateTime.MinValue || 
            fechaUltimoMes.Year != ahora.Year || 
            fechaUltimoMes.Month != ahora.Month)
        {
            viajesMensuales = 0;
            fechaUltimoMes = ahora;
        }
    }

    public virtual bool PuedeUsarTrasbordo(string lineaColectivo)
    {
        DateTime ahora = tiempoProvider.Ahora;
        
        // Verificar si pasó menos de 1 hora desde el último boleto
        if (ultimoBoletoParaTrasbordo != DateTime.MinValue)
        {
            TimeSpan tiempoTranscurrido = ahora - ultimoBoletoParaTrasbordo;
            
            // Debe ser menos de 1 hora, línea diferente a la última, y NO haber sido usada antes en esta cadena
            if (tiempoTranscurrido.TotalHours < 1 && 
                lineaColectivo != ultimaLineaUsada && 
                !lineasUsadasEnTrasbordo.Contains(lineaColectivo))
            {
                // Verificar horario: lunes a sábado de 7:00 a 22:00
                if (ahora.DayOfWeek >= DayOfWeek.Monday && ahora.DayOfWeek <= DayOfWeek.Saturday)
                {
                    if (ahora.Hour >= 7 && ahora.Hour < 22)
                    {
                        return true;
                    }
                }
            }
        }
        
        return false;
    }

    public void RegistrarTrasbordo(string lineaColectivo)
    {
        ultimaLineaUsada = lineaColectivo;
        ultimoViaje = tiempoProvider.Ahora;
        
        // Agregar la línea a la lista de líneas usadas en esta cadena de trasbordos
        if (!lineasUsadasEnTrasbordo.Contains(lineaColectivo))
        {
            lineasUsadasEnTrasbordo.Add(lineaColectivo);
        }
    }
}
