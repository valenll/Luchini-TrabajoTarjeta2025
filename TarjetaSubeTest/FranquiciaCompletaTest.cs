/*using System;
using NUnit.Framework;

[TestFixture]
public class TarjetaFranquiciaCompletaTests
{
    private TarjetaFranquiciaCompleta tarjeta;
    private Colectivo colectivo;
    private TiempoSimulado tiempo;

    [SetUp]
    public void Setup()
    {
        // Lunes 25 de noviembre de 2024 a las 10:00 AM (horario permitido)
        tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
        tarjeta = new TarjetaFranquiciaCompleta(tiempo);
        colectivo = new Colectivo("120", "Rosario Bus", tiempo);
    }

    [Test]
    public void Test_ObtenerTarifa_DebeRetornarCero()
    {
        decimal tarifa = tarjeta.ObtenerTarifa();
        Assert.AreEqual(0m, tarifa, "La tarifa debe ser $0 para franquicia completa");
    }

    [Test]
    public void Test_ObtenerTipo_DebeRetornarFranquiciaCompleta()
    {
        string tipo = tarjeta.ObtenerTipo();
        Assert.AreEqual("Franquicia Completa", tipo);
    }

    [Test]
    public void Test_PrimerViaje_DebeSerGratuito()
    {
        decimal saldoInicial = tarjeta.Saldo;

        bool resultado = tarjeta.PagarPasaje();
        Boleto boleto = colectivo.PagarCon(tarjeta);

        Assert.IsTrue(resultado, "El primer viaje debe ser exitoso");
        Assert.IsNotNull(boleto, "Debe generar un boleto");
        Assert.AreEqual(0m, boleto.MontoPagado, "El monto pagado debe ser $0");
        Assert.AreEqual(saldoInicial, tarjeta.Saldo, "El saldo no debe cambiar");
    }

    [Test]
    public void Test_SegundoViaje_DebeSerGratuito()
    {
        colectivo.PagarCon(tarjeta); // Primer viaje
        decimal saldoAntes = tarjeta.Saldo;

        Boleto boleto = colectivo.PagarCon(tarjeta); // Segundo viaje

        Assert.IsNotNull(boleto, "Debe generar un boleto");
        Assert.AreEqual(0m, boleto.MontoPagado, "El monto pagado debe ser $0");
        Assert.AreEqual(saldoAntes, tarjeta.Saldo, "El saldo no debe cambiar");
    }

    [Test]
    public void Test_NoSePuedenRealizarMasDeDosViajesGratuitosPorDia()
    {
        tarjeta.Cargar(5000m);
        
        Boleto boleto1 = colectivo.PagarCon(tarjeta); // Primer viaje gratuito
        Boleto boleto2 = colectivo.PagarCon(tarjeta); // Segundo viaje gratuito

        Assert.IsNotNull(boleto1, "El primer viaje debe ser exitoso");
        Assert.IsNotNull(boleto2, "El segundo viaje debe ser exitoso");
        Assert.AreEqual(0m, boleto1.MontoPagado, "El primer viaje debe ser gratuito");
        Assert.AreEqual(0m, boleto2.MontoPagado, "El segundo viaje debe ser gratuito");
        
        // Tercer viaje debe cobrar tarifa completa
        Boleto boleto3 = colectivo.PagarCon(tarjeta);

        Assert.IsNotNull(boleto3, "El tercer viaje debe ser exitoso pero cobrado");
        Assert.AreEqual(1580m, boleto3.MontoPagado, 
            "El tercer viaje debe cobrar tarifa completa");
    }

    [Test]
    public void Test_ViajesPosterioresAlSegundo_SeCobranConPrecioCompleto()
    {
        const decimal TARIFA_COMPLETA = 1580m;
        
        tarjeta.Cargar(10000m);
        decimal saldoInicial = tarjeta.Saldo;
        
        // Realizar dos viajes gratuitos
        Boleto boleto1 = colectivo.PagarCon(tarjeta);
        Boleto boleto2 = colectivo.PagarCon(tarjeta);
        
        Assert.IsNotNull(boleto1, "El primer viaje debe ser exitoso");
        Assert.IsNotNull(boleto2, "El segundo viaje debe ser exitoso");
        Assert.AreEqual(0m, boleto1.MontoPagado, "El primer viaje debe ser gratuito");
        Assert.AreEqual(0m, boleto2.MontoPagado, "El segundo viaje debe ser gratuito");
        Assert.AreEqual(saldoInicial, tarjeta.Saldo, "El saldo no debe cambiar");
        
        decimal saldoAntesTercerViaje = tarjeta.Saldo;

        // Tercer viaje debe cobrar tarifa completa
        Boleto boleto3 = colectivo.PagarCon(tarjeta);

        Assert.IsNotNull(boleto3, "El tercer viaje debe ser exitoso");
        Assert.AreEqual(TARIFA_COMPLETA, boleto3.MontoPagado, 
            "El tercer viaje debe cobrar la tarifa completa ($1580)");
        Assert.AreEqual(saldoAntesTercerViaje - TARIFA_COMPLETA, tarjeta.Saldo, 
            "Debe descontarse la tarifa completa del saldo");
        Assert.AreEqual("Franquicia Completa", boleto3.TipoTarjeta, 
            "El tipo debe seguir siendo 'Franquicia Completa'");
        
        // Cuarto viaje también debe cobrar tarifa completa
        decimal saldoAntesCuartoViaje = tarjeta.Saldo;
        Boleto boleto4 = colectivo.PagarCon(tarjeta);
        
        Assert.IsNotNull(boleto4, "El cuarto viaje debe ser exitoso");
        Assert.AreEqual(TARIFA_COMPLETA, boleto4.MontoPagado, 
            "El cuarto viaje también debe cobrar la tarifa completa ($1580)");
        Assert.AreEqual(saldoAntesCuartoViaje - TARIFA_COMPLETA, tarjeta.Saldo, 
            "Debe descontarse la tarifa completa del saldo");
    }

    [Test]
    public void Test_ContadorDeViajesSeReiniciaNuevoDia()
    {
        tarjeta.Cargar(10000m);
        
        // Dos viajes gratuitos
        colectivo.PagarCon(tarjeta);
        colectivo.PagarCon(tarjeta);

        // Tercer viaje - tarifa completa
        Boleto tercerViajeMismoDia = colectivo.PagarCon(tarjeta);
        Assert.IsNotNull(tercerViajeMismoDia);
        Assert.AreEqual(1580m, tercerViajeMismoDia.MontoPagado);
        
        // Avanzar al día siguiente (horario permitido)
        tiempo.EstablecerTiempo(new DateTime(2024, 11, 26, 10, 0, 0));
        
        // Primer viaje del nuevo día - debe ser gratis otra vez
        Boleto primerViajeNuevoDia = colectivo.PagarCon(tarjeta);
        Assert.IsNotNull(primerViajeNuevoDia);
        Assert.AreEqual(0m, primerViajeNuevoDia.MontoPagado, 
            "El primer viaje del nuevo día debe ser gratuito");
    }

    [Test]
    public void Test_PrimerViajeDelDia_SinViajesPrevios()
    {
        TarjetaFranquiciaCompleta tarjetaNueva = new TarjetaFranquiciaCompleta(tiempo);

        bool resultado = tarjetaNueva.PagarPasaje();

        Assert.IsTrue(resultado, "El primer viaje debe ser exitoso");
    }

    [Test]
    public void Test_DosViajesConsecutivos_AmbosGratuitos()
    {
        Boleto primerViaje = colectivo.PagarCon(tarjeta);
        Boleto segundoViaje = colectivo.PagarCon(tarjeta);

        Assert.IsNotNull(primerViaje, "El primer viaje debe ser exitoso");
        Assert.IsNotNull(segundoViaje, "El segundo viaje debe ser exitoso");
        Assert.AreEqual(0m, primerViaje.MontoPagado, "Primer viaje debe ser gratuito");
        Assert.AreEqual(0m, segundoViaje.MontoPagado, "Segundo viaje debe ser gratuito");
        Assert.AreEqual(0m, tarjeta.Saldo, "El saldo debe permanecer en 0");
    }

    [Test]
    public void Test_MultiplesViajesPostLimite_TodosSeCobraNormal()
    {
        tarjeta.Cargar(20000m);
        
        colectivo.PagarCon(tarjeta); // Viaje 1 - Gratis
        colectivo.PagarCon(tarjeta); // Viaje 2 - Gratis
        
        decimal saldoAntes = tarjeta.Saldo;

        // Múltiples viajes posteriores
        Boleto viaje3 = colectivo.PagarCon(tarjeta);
        Boleto viaje4 = colectivo.PagarCon(tarjeta);
        Boleto viaje5 = colectivo.PagarCon(tarjeta);

        Assert.IsNotNull(viaje3, "El tercer viaje debe ser exitoso");
        Assert.IsNotNull(viaje4, "El cuarto viaje debe ser exitoso");
        Assert.IsNotNull(viaje5, "El quinto viaje debe ser exitoso");
        
        Assert.AreEqual(1580m, viaje3.MontoPagado, "El tercer viaje debe cobrar tarifa completa");
        Assert.AreEqual(1580m, viaje4.MontoPagado, "El cuarto viaje debe cobrar tarifa completa");
        Assert.AreEqual(1580m, viaje5.MontoPagado, "El quinto viaje debe cobrar tarifa completa");
        
        Assert.AreEqual(saldoAntes - (3 * 1580m), tarjeta.Saldo, 
            "Deben haberse descontado 3 tarifas completas");
    }

    [Test]
    public void Test_BoletoGenerado_ContieneInformacionCorrecta()
    {
        Boleto boleto = colectivo.PagarCon(tarjeta);

        Assert.IsNotNull(boleto, "Debe generar un boleto");
        Assert.AreEqual(0m, boleto.MontoPagado, "El monto debe ser $0");
        Assert.AreEqual("Franquicia Completa", boleto.TipoTarjeta);
        Assert.AreEqual("120", boleto.LineaColectivo);
        Assert.AreEqual("Rosario Bus", boleto.Empresa);
        Assert.AreEqual(new DateTime(2024, 11, 25, 10, 0, 0), boleto.FechaHora);
    }

    [Test]
    public void Test_ViajesPosteriores_PuedenUsarSaldoNegativo()
    {
        tarjeta.Cargar(2000m);
        
        // Dos viajes gratuitos
        colectivo.PagarCon(tarjeta);
        colectivo.PagarCon(tarjeta);
        
        Assert.AreEqual(2000m, tarjeta.Saldo);
        
        // Tercer viaje: 2000 - 1580 = 420
        Boleto boleto3 = colectivo.PagarCon(tarjeta);
        Assert.IsNotNull(boleto3);
        Assert.AreEqual(420m, tarjeta.Saldo);
        
        // Cuarto viaje: 420 - 1580 = -1160 (usa saldo negativo)
        Boleto boleto4 = colectivo.PagarCon(tarjeta);
        Assert.IsNotNull(boleto4);
        Assert.AreEqual(-1160m, tarjeta.Saldo);
    }

    [Test]
    public void Test_ViajesPosteriores_RespetanLimiteSaldoNegativo()
    {
        // Sin cargar saldo
        // Dos viajes gratuitos
        colectivo.PagarCon(tarjeta);
        colectivo.PagarCon(tarjeta);
        
        // Tercer viaje: 0 - 1580 = -1580 (supera límite de -1200)
        Boleto boleto3 = colectivo.PagarCon(tarjeta);
        
        Assert.IsNull(boleto3, "No debe permitir viaje que supere el límite de saldo negativo");
        Assert.AreEqual(0m, tarjeta.Saldo);
    }

    [Test]
    public void Test_FueraDeHorarioPermitido_NoPermiteViaje()
    {
        // Cambiar a sábado 5:00 (fuera de horario)
        tiempo.EstablecerTiempo(new DateTime(2024, 11, 23, 5, 0, 0));
        
        Boleto boleto = colectivo.PagarCon(tarjeta);
        
        Assert.IsNull(boleto, "No debe permitir viajes fuera del horario permitido");
    }

    [Test]
    public void Test_DomingoEnHorarioPermitido_NoPermiteViaje()
    {
        // Cambiar a domingo 10:00
        tiempo.EstablecerTiempo(new DateTime(2024, 11, 24, 10, 0, 0));
        
        Boleto boleto = colectivo.PagarCon(tarjeta);
        
        Assert.IsNull(boleto, "No debe permitir viajes en domingo");
    }
}*/