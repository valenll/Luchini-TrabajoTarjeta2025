using System;
public class TiempoSimulado : ITiempoProvider
{
    private DateTime tiempoActual;

    public TiempoSimulado(DateTime tiempoInicial)
    {
        tiempoActual = tiempoInicial;
    }

    public TiempoSimulado() : this(DateTime.Now)
    {
    }

    public DateTime Ahora => tiempoActual;

    public void EstablecerTiempo(DateTime tiempo)
    {
        tiempoActual = tiempo;
    }

    public void AvanzarTiempo(TimeSpan intervalo)
    {
        tiempoActual = tiempoActual.Add(intervalo);
    }

    public void AvanzarMinutos(double minutos)
    {
        AvanzarTiempo(TimeSpan.FromMinutes(minutos));
    }

    public void AvanzarHoras(double horas)
    {
        AvanzarTiempo(TimeSpan.FromHours(horas));
    }
    
    public void AvanzarDias(int dias)
    {
        AvanzarTiempo(TimeSpan.FromDays(dias));
    }
}
