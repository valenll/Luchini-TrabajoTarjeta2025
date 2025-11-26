using System;

public class TarjetaMedioBoleto : Tarjeta
{
    private const decimal DESCUENTO = 0.5m;
    private const double MINUTOS_ESPERA = 5.0;
    private const int MAX_VIAJES_MEDIO_BOLETO = 2;

    public TarjetaMedioBoleto() : base()
    {
    }

    public TarjetaMedioBoleto(ITiempoProvider tiempoProvider) : base(tiempoProvider)
    {
    }

    public override bool PagarPasaje(string lineaColectivo = "", bool esInterurbano = false)
    {
        // Validar franja horaria: Lunes a viernes de 6 a 22
        if (!EstaEnFranjaHorariaPermitida())
        {
            return false;
        }
        
        ActualizarContadorViajes();

        // Validar 5 minutos solo para viajes con medio boleto (primeros 2 del día)
        if (viajesHoy < MAX_VIAJES_MEDIO_BOLETO)
        {
            if (ultimoViaje != DateTime.MinValue)
            {
                TimeSpan tiempoTranscurrido = tiempoProvider.Ahora - ultimoViaje;
                if (tiempoTranscurrido.TotalMinutes < MINUTOS_ESPERA)
                {
                    return false;
                }
            }
        }

        ActualizarContadorViajesMensuales();
        
        // Obtener tarifa ANTES de incrementar los contadores
        decimal tarifaAPagar = ObtenerTarifa(esInterurbano);
        
        // DESPUÉS incrementar
        viajesHoy++;
        viajesMensuales++;
        
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
            // Si falla el pago, revertir los incrementos
            viajesHoy--;
            viajesMensuales--;
        }
        return resultado;
    }

    public override decimal ObtenerTarifa(bool esInterurbano = false)
    {
        ActualizarContadorViajes();
        
        if (esInterurbano)
        {
            // Para interurbano, aplicar el descuento sobre la tarifa interurbana
            if (viajesHoy < MAX_VIAJES_MEDIO_BOLETO)
            {
                return 3000m * DESCUENTO;
            }
            return 3000m;
        }
        
        // Los primeros 2 viajes del día son con medio boleto
        if (viajesHoy < MAX_VIAJES_MEDIO_BOLETO)
        {
            return base.ObtenerTarifa(false) * DESCUENTO;
        }
        
        // Del tercer viaje en adelante se cobra tarifa completa
        return base.ObtenerTarifa(false);
    }

    public override string ObtenerTipo()
    {
        return "Medio Boleto";
    }

    private bool EstaEnFranjaHorariaPermitida()
    {
        DateTime ahora = tiempoProvider.Ahora;
        
        // Lunes a viernes
        if (ahora.DayOfWeek >= DayOfWeek.Monday && ahora.DayOfWeek <= DayOfWeek.Friday)
        {
            // De 6 a 22 horas
            if (ahora.Hour >= 6 && ahora.Hour < 22)
            {
                return true;
            }
        }
        
        return false;
    }
}
