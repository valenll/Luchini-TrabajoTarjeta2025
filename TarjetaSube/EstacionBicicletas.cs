using System;
using System.Collections.Generic;

public class EstacionBicicletas
{
    private const decimal TARIFA_DIARIA = 1777.50m;
    private const decimal MULTA_POR_HORA = 1000m;
    private const int TIEMPO_MAXIMO_USO_MINUTOS = 60;

    private ITiempoProvider tiempoProvider;
    private Dictionary<int, List<RetiroBici>> historialRetiros; // Historial segun ID de tarjeta

    public EstacionBicicletas() : this(new TiempoReal())
    {
    }

    public EstacionBicicletas(ITiempoProvider tiempoProvider)
    {
        this.tiempoProvider = tiempoProvider;
        this.historialRetiros = new Dictionary<int, List<RetiroBici>>();
    }

    public RetiroBici RetirarBici(Tarjeta tarjeta)
    {
        if (tarjeta == null)
            return null;

        // Calcular multas pendientes
        decimal multasPendientes = CalcularMultasPendientes(tarjeta.Id);
        
        // Total a pagar = tarifa diaria + multas pendientes
        decimal totalAPagar = TARIFA_DIARIA + multasPendientes;

        // Intentar descontar el monto total
        if (!tarjeta.Descontar(totalAPagar))
        {
            // No hay saldo suficiente
            return null;
        }

        // Crear el registro del retiro
        RetiroBici retiro = new RetiroBici(
            tarjeta.Id,
            tiempoProvider.Ahora,
            TARIFA_DIARIA,
            multasPendientes
        );

        // Guardar en el historial
        if (!historialRetiros.ContainsKey(tarjeta.Id))
        {
            historialRetiros[tarjeta.Id] = new List<RetiroBici>();
        }
        historialRetiros[tarjeta.Id].Add(retiro);
        
        // Marcar las multas anteriores como cobradas
        if (multasPendientes > 0)
        {
            MarcarMultasComoCobradas(tarjeta.Id);
        }

        return retiro;
    }

    public void DevolverBici(int idTarjeta)
    {
        if (!historialRetiros.ContainsKey(idTarjeta))
            return;

        List<RetiroBici> retiros = historialRetiros[idTarjeta];
        
        // Buscar el último retiro sin devolución
        for (int i = retiros.Count - 1; i >= 0; i--)
        {
            if (retiros[i].HoraDevolucion == DateTime.MinValue)
            {
                retiros[i].RegistrarDevolucion(tiempoProvider.Ahora);
                break;
            }
        }
    }

    public decimal CalcularMultasPendientes(int idTarjeta)
    {
        if (!historialRetiros.ContainsKey(idTarjeta))
            return 0m;

        DateTime hoy = tiempoProvider.Ahora.Date;
        decimal multasAcumuladas = 0m;

        foreach (RetiroBici retiro in historialRetiros[idTarjeta])
        {
            // Solo contar multas del día actual, devueltas y no cobradas
            if (retiro.HoraRetiro.Date == hoy && 
                retiro.HoraDevolucion != DateTime.MinValue &&
                !retiro.MultaCobrada)
            {
                TimeSpan tiempoUso = retiro.HoraDevolucion - retiro.HoraRetiro;
                
                if (tiempoUso.TotalMinutes > TIEMPO_MAXIMO_USO_MINUTOS)
                {
                    // Por cada hora extra se cobra una multa
                    int horasExtra = (int)Math.Ceiling((tiempoUso.TotalMinutes - TIEMPO_MAXIMO_USO_MINUTOS) / 60.0);
                    multasAcumuladas += horasExtra * MULTA_POR_HORA;
                }
            }
        }

        return multasAcumuladas;
    }

    private void MarcarMultasComoCobradas(int idTarjeta)
    {
        if (!historialRetiros.ContainsKey(idTarjeta))
            return;

        DateTime hoy = tiempoProvider.Ahora.Date;

        foreach (RetiroBici retiro in historialRetiros[idTarjeta])
        {
            if (retiro.HoraRetiro.Date == hoy && 
                retiro.HoraDevolucion != DateTime.MinValue &&
                !retiro.MultaCobrada)
            {
                TimeSpan tiempoUso = retiro.HoraDevolucion - retiro.HoraRetiro;
                if (tiempoUso.TotalMinutes > TIEMPO_MAXIMO_USO_MINUTOS)
                {
                    retiro.MarcarMultaCobrada();
                }
            }
        }
    }

    public List<RetiroBici> ObtenerHistorial(int idTarjeta)
    {
        if (!historialRetiros.ContainsKey(idTarjeta))
            return new List<RetiroBici>();

        return new List<RetiroBici>(historialRetiros[idTarjeta]);
    }

    public bool TieneBiciSinDevolver(int idTarjeta)
    {
        if (!historialRetiros.ContainsKey(idTarjeta))
            return false;

        foreach (RetiroBici retiro in historialRetiros[idTarjeta])
        {
            if (retiro.HoraDevolucion == DateTime.MinValue)
                return true;
        }

        return false;
    }
}

public class RetiroBici
{
    private int idTarjeta;
    private DateTime horaRetiro;
    private DateTime horaDevolucion;
    private decimal tarifaCobrada;
    private decimal multasCobradas;
    private bool multaCobrada;

    public RetiroBici(int idTarjeta, DateTime horaRetiro, decimal tarifaCobrada, decimal multasCobradas)
    {
        this.idTarjeta = idTarjeta;
        this.horaRetiro = horaRetiro;
        this.horaDevolucion = DateTime.MinValue; // Sin devolver aún
        this.tarifaCobrada = tarifaCobrada;
        this.multasCobradas = multasCobradas;
        this.multaCobrada = false;
    }

    public int IdTarjeta
    {
        get { return idTarjeta; }
    }

    public DateTime HoraRetiro
    {
        get { return horaRetiro; }
    }

    public DateTime HoraDevolucion
    {
        get { return horaDevolucion; }
    }

    public decimal TarifaCobrada
    {
        get { return tarifaCobrada; }
    }

    public decimal MultasCobradas
    {
        get { return multasCobradas; }
    }

    public decimal TotalCobrado
    {
        get { return tarifaCobrada + multasCobradas; }
    }

    public bool MultaCobrada
    {
        get { return multaCobrada; }
    }

    public void RegistrarDevolucion(DateTime hora)
    {
        if (horaDevolucion == DateTime.MinValue)
        {
            horaDevolucion = hora;
        }
    }

    public void MarcarMultaCobrada()
    {
        multaCobrada = true;
    }

    public bool FueDevuelta
    {
        get { return horaDevolucion != DateTime.MinValue; }
    }

    public TimeSpan TiempoDeUso
    {
        get
        {
            if (horaDevolucion == DateTime.MinValue)
                return TimeSpan.Zero;
            return horaDevolucion - horaRetiro;
        }
    }

    public override string ToString()
    {
        string devolucionInfo = horaDevolucion != DateTime.MinValue 
            ? $"Devuelta: {horaDevolucion:dd/MM/yyyy HH:mm} (Uso: {TiempoDeUso.TotalMinutes:F0} min)"
            : "Sin devolver";

        return $"Retiro Bici - Tarjeta ID: {idTarjeta}\n" +
               $"Retirada: {horaRetiro:dd/MM/yyyy HH:mm}\n" +
               $"{devolucionInfo}\n" +
               $"Tarifa: ${tarifaCobrada}\n" +
               $"Multas: ${multasCobradas}\n" +
               $"Total cobrado: ${TotalCobrado}";
    }
}
