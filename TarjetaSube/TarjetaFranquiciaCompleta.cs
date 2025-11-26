using System;

public class TarjetaFranquiciaCompleta : Tarjeta
{
    private const int MAX_VIAJES_DIARIOS = 2;

    public TarjetaFranquiciaCompleta() : base()
    {
    }

    public TarjetaFranquiciaCompleta(ITiempoProvider tiempoProvider) : base(tiempoProvider)
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
        ActualizarContadorViajesMensuales();

        if (viajesHoy < MAX_VIAJES_DIARIOS)
        {
            // Primeros 2 viajes del día: GRATIS
            viajesHoy++;
            viajesMensuales++;
            DateTime ahora = tiempoProvider.Ahora;
            ultimoViaje = ahora;
            ultimoBoletoParaTrasbordo = ahora;
            ultimaLineaUsada = lineaColectivo;
            return true; // No descuenta saldo
        }
        else
        {
            // Viajes posteriores al segundo: COBRAR TARIFA COMPLETA
            viajesHoy++;
            viajesMensuales++;
            
            decimal tarifaAPagar = base.ObtenerTarifa(esInterurbano);
            
            bool resultado = Descontar(tarifaAPagar);
            if (resultado)
            {
                DateTime ahora = tiempoProvider.Ahora;
                ultimoViaje = ahora;
                ultimoBoletoParaTrasbordo = ahora;
                ultimaLineaUsada = lineaColectivo;
            }
            else
            {
                // Si falla el pago, revertir los incrementos
                viajesHoy--;
                viajesMensuales--;
            }
            return resultado;
        }
    }

    public override decimal ObtenerTarifa(bool esInterurbano = false)
    {
        ActualizarContadorViajes();
        
        if (viajesHoy < MAX_VIAJES_DIARIOS)
        {
            return 0m; // Gratis para los primeros 2 viajes
        }
        else
        {
            return base.ObtenerTarifa(esInterurbano); // Tarifa completa para viajes posteriores
        }
    }

    public override string ObtenerTipo()
    {
        return "Franquicia Completa";
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
