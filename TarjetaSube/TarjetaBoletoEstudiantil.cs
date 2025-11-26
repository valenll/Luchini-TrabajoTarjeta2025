public class TarjetaBoletoGratuitoEstudiantil : TarjetaFranquiciaCompleta
{
    public TarjetaBoletoGratuitoEstudiantil() : base()
    {
    }

    public TarjetaBoletoGratuitoEstudiantil(ITiempoProvider tiempoProvider) : base(tiempoProvider)
    {
    }
    
    //hereda el comportamiento de la franquicia completa (gratuito completo)
}
