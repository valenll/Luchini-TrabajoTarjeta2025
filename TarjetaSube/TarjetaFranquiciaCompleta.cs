using System;

public class TarjetaFranquiciaCompleta : Tarjeta
{
    private const int MAX_VIAJES_DIARIOS = 2;

    public override bool PagarPasaje()
    {
        ActualizarContadorViajes();

        if (viajesHoy < MAX_VIAJES_DIARIOS)
        {
            // Primeros 2 viajes del día: GRATIS
            viajesHoy++;
            ultimoViaje = DateTime.Now;
            return true; // No descuenta saldo
        }
        else
        {
            // Viajes posteriores al segundo: COBRAR TARIFA COMPLETA
            bool resultado = Descontar(base.ObtenerTarifa());
            if (resultado)
            {
                viajesHoy++;
                ultimoViaje = DateTime.Now;
            }
            return resultado;
        }
    }

    public override decimal ObtenerTarifa()
    {
        ActualizarContadorViajes();
        
        if (viajesHoy < MAX_VIAJES_DIARIOS)
        {
            return 0m; // Gratis para los primeros 2 viajes
        }
        else
        {
            return base.ObtenerTarifa(); // Tarifa completa para viajes posteriores
        }
    }

    public override string ObtenerTipo()
    {
        return "Franquicia Completa";
    }
}
