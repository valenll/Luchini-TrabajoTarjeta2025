public class Colectivo
{
    private string linea;
    private string empresa;
    protected ITiempoProvider tiempoProvider;

    public Colectivo(string linea, string empresa) : this(linea, empresa, new TiempoReal())
    {
    }

    public Colectivo(string linea, string empresa, ITiempoProvider tiempoProvider)
    {
        this.linea = linea;
        this.empresa = empresa;
        this.tiempoProvider = tiempoProvider;
    }

    public virtual Boleto PagarCon(Tarjeta tarjeta)
    {
        if (tarjeta == null)
            return null;

        // Verificar si puede usar trasbordo
        bool esTrasbordo = tarjeta.PuedeUsarTrasbordo(linea);

        if (esTrasbordo)
        {
            // Registrar el trasbordo sin cobrar
            tarjeta.RegistrarTrasbordo(linea);
            return new Boleto(0m, linea, empresa, tarjeta.Saldo, tarjeta.ObtenerTipo(), tarjeta.Id, true, tiempoProvider);
        }

        decimal tarifaAPagar = tarjeta.ObtenerTarifa(false); // false = no es interurbano

        // Intentar pagar el pasaje
        if (tarjeta.PagarPasaje(linea, false)) // false = no es interurbano
        {
            return new Boleto(tarifaAPagar, linea, empresa, tarjeta.Saldo, tarjeta.ObtenerTipo(), tarjeta.Id, false, tiempoProvider);
        }

        return null;
    }

    public string Linea
    {
        get { return linea; }
    }

    public string Empresa
    {
        get { return empresa; }
    }
}
