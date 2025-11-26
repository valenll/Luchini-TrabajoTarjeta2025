using System;

/// <summary>
/// Implementación del proveedor de tiempo que permite controlar manualmente
/// la fecha y hora. Se usa principalmente en tests.
/// </summary>
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

    /// <summary>
    /// Establece manualmente la fecha y hora actual.
    /// </summary>
    public void EstablecerTiempo(DateTime tiempo)
    {
        tiempoActual = tiempo;
    }

    /// <summary>
    /// Avanza el tiempo simulado por un intervalo específico.
    /// </summary>
    public void AvanzarTiempo(TimeSpan intervalo)
    {
        tiempoActual = tiempoActual.Add(intervalo);
    }

    /// <summary>
    /// Avanza el tiempo simulado por una cantidad de minutos.
    /// </summary>
    public void AvanzarMinutos(double minutos)
    {
        AvanzarTiempo(TimeSpan.FromMinutes(minutos));
    }

    /// <summary>
    /// Avanza el tiempo simulado por una cantidad de horas.
    /// </summary>
    public void AvanzarHoras(double horas)
    {
        AvanzarTiempo(TimeSpan.FromHours(horas));
    }

    /// <summary>
    /// Avanza el tiempo simulado por una cantidad de días.
    /// </summary>
    public void AvanzarDias(int dias)
    {
        AvanzarTiempo(TimeSpan.FromDays(dias));
    }
}
