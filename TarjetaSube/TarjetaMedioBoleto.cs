using System;

public class TarjetaMedioBoleto : Tarjeta
{
    private const decimal DESCUENTO = 0.5m;
    private const double MINUTOS_ESPERA = 5.0; // Cambiado a double para usar con TotalMinutes
    private const int MAX_VIAJES_MEDIO_BOLETO = 2;

    public override bool PagarPasaje()
    {
        ActualizarContadorViajes();

        // Validar 5 minutos solo para viajes con medio boleto (primeros 2 del día)
        if (viajesHoy < MAX_VIAJES_MEDIO_BOLETO)
        {
            if (ultimoViaje != DateTime.MinValue)
            {
                TimeSpan tiempoTranscurrido = DateTime.Now - ultimoViaje;
                if (tiempoTranscurrido.TotalMinutes < MINUTOS_ESPERA)
                {
                    return false;
                }
            }
        }

        bool resultado = Descontar(ObtenerTarifa());
        if (resultado)
        {
            ultimoViaje = DateTime.Now;
            viajesHoy++;
        }
        return resultado;
    }

    public override decimal ObtenerTarifa()
    {
        ActualizarContadorViajes();
        
        // Los primeros 2 viajes del día son con medio boleto
        if (viajesHoy < MAX_VIAJES_MEDIO_BOLETO)
        {
            return base.ObtenerTarifa() * DESCUENTO;
        }
        
        // Del tercer viaje en adelante se cobra tarifa completa
        return base.ObtenerTarifa();
    }

    public override string ObtenerTipo()
    {
        return "Medio Boleto";
    }
}