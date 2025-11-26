using System;
public class TiempoReal : ITiempoProvider
{
    public DateTime Ahora => DateTime.Now;
}
