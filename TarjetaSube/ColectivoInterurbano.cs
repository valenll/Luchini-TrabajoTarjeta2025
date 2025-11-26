public class ColectivoInterurbano : Colectivo
{
    public ColectivoInterurbano(string linea, string empresa) : base(linea, empresa)
    {
    }

    public ColectivoInterurbano(string linea, string empresa, ITiempoProvider tiempoProvider) : base(linea, empresa, tiempoProvider)
    {
    }

    public override Boleto PagarCon(Tarjeta tarjeta)
    {
        if (tarjeta == null)
            return null;

        // Verificar si puede usar trasbordo
        bool esTrasbordo = tarjeta.PuedeUsarTrasbordo(Linea);

        if (esTrasbordo)
        {
            // Registrar el trasbordo sin cobrar
            tarjeta.RegistrarTrasbordo(Linea);
            return new Boleto(0m, Linea, Empresa, tarjeta.Saldo, tarjeta.ObtenerTipo(), tarjeta.Id, true, tiempoProvider);
        }

        decimal tarifaAPagar = tarjeta.ObtenerTarifa(true); // true = es interurbano

        // Intentar pagar el pasaje
        if (tarjeta.PagarPasaje(Linea, true)) // true = es interurbano
        {
            return new Boleto(tarifaAPagar, Linea, Empresa, tarjeta.Saldo, tarjeta.ObtenerTipo(), tarjeta.Id, false, tiempoProvider);
        }

        return null;
    }
}
