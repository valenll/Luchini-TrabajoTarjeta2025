using System;

/// <summary>
/// Interfaz que abstrae la obtención del tiempo actual.
/// Permite inyectar diferentes implementaciones para producción y tests.
/// </summary>
public interface ITiempoProvider
{
    /// <summary>
    /// Obtiene la fecha y hora actual.
    /// </summary>
    DateTime Ahora { get; }
}
