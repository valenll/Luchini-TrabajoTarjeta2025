using System;
using System.Linq;

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

    public Tarjeta()
    {
        saldo = 0m;
        saldoPendiente = 0m;
        ultimoViaje = DateTime.MinValue;
        viajesHoy = 0;
        fechaUltimoDia = DateTime.MinValue;
        id = ++contadorId;
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

    public virtual bool PagarPasaje()
    {
        bool resultado = Descontar(TARIFA_BASICA);
        if (resultado)
        {
            ultimoViaje = DateTime.Now;
        }
        return resultado;
    }

    public virtual decimal ObtenerTarifa()
    {
        return TARIFA_BASICA;
    }

    public virtual string ObtenerTipo()
    {
        return "Normal";
    }

    protected void ActualizarContadorViajes()
    {
        DateTime ahora = DateTime.Now;
        if (fechaUltimoDia.Date != ahora.Date)
        {
            viajesHoy = 0;
            fechaUltimoDia = ahora;
        }
    }
}