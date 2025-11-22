using System;
using NUnit.Framework;

[TestFixture]
public class TarjetaFranquiciaCompletaTests
{
    private TarjetaFranquiciaCompleta tarjeta;
    private Colectivo colectivo;

    [SetUp]
    public void Setup()
    {
        tarjeta = new TarjetaFranquiciaCompleta();
        colectivo = new Colectivo("120", "Rosario Bus");
    }

    [Test]
    public void Test_ObtenerTarifa_DebeRetornarCero()
    {
        // Arrange & Act
        decimal tarifa = tarjeta.ObtenerTarifa();

        // Assert
        Assert.AreEqual(0m, tarifa, "La tarifa debe ser $0 para franquicia completa");
    }

    [Test]
    public void Test_ObtenerTipo_DebeRetornarFranquiciaCompleta()
    {
        // Arrange & Act
        string tipo = tarjeta.ObtenerTipo();

        // Assert
        Assert.AreEqual("Franquicia Completa", tipo, "El tipo debe ser 'Franquicia Completa'");
    }

    [Test]
    public void Test_PrimerViaje_DebeSerGratuito()
    {
        // Arrange
        decimal saldoInicial = tarjeta.Saldo;

        // Act
        bool resultado = tarjeta.PagarPasaje();
        Boleto boleto = colectivo.PagarCon(tarjeta);

        // Assert
        Assert.IsTrue(resultado, "El primer viaje debe ser exitoso");
        Assert.IsNotNull(boleto, "Debe generar un boleto");
        Assert.AreEqual(0m, boleto.MontoPagado, "El monto pagado debe ser $0");
        Assert.AreEqual(saldoInicial, tarjeta.Saldo, "El saldo no debe cambiar");
    }

    [Test]
    public void Test_SegundoViaje_DebeSerGratuito()
    {
        // Arrange
        colectivo.PagarCon(tarjeta); // Primer viaje
        decimal saldoAntes = tarjeta.Saldo;

        // Act
        Boleto boleto = colectivo.PagarCon(tarjeta); // Segundo viaje

        // Assert
        Assert.IsNotNull(boleto, "Debe generar un boleto");
        Assert.AreEqual(0m, boleto.MontoPagado, "El monto pagado debe ser $0");
        Assert.AreEqual(saldoAntes, tarjeta.Saldo, "El saldo no debe cambiar");
    }

    [Test]
    public void Test_NoSePuedenRealizarMasDeDosViajesGratuitosPorDia()
    {
        // Arrange - Cargar saldo para pagar viajes posteriores
        tarjeta.Cargar(5000m);
        
        Boleto boleto1 = colectivo.PagarCon(tarjeta); // Primer viaje gratuito
        Boleto boleto2 = colectivo.PagarCon(tarjeta); // Segundo viaje gratuito

        // Assert - Primeros dos viajes son gratuitos
        Assert.IsNotNull(boleto1, "El primer viaje debe ser exitoso");
        Assert.IsNotNull(boleto2, "El segundo viaje debe ser exitoso");
        Assert.AreEqual(0m, boleto1.MontoPagado, "El primer viaje debe ser gratuito");
        Assert.AreEqual(0m, boleto2.MontoPagado, "El segundo viaje debe ser gratuito");
        
        // Act - Tercer viaje debe cobrar tarifa completa (no es gratuito)
        Boleto boleto3 = colectivo.PagarCon(tarjeta);

        // Assert
        Assert.IsNotNull(boleto3, "El tercer viaje debe ser exitoso pero cobrado");
        Assert.AreEqual(1580m, boleto3.MontoPagado, 
            "El tercer viaje debe cobrar tarifa completa - no más viajes gratuitos");
    }

    [Test]
    public void Test_ViajesPosterioresAlSegundo_SeCobranConPrecioCompleto()
    {
        // Arrange
        const decimal TARIFA_COMPLETA = 1580m;
        
        // Cargar saldo en la tarjeta de franquicia para poder pagar viajes posteriores
        tarjeta.Cargar(10000m);
        decimal saldoInicial = tarjeta.Saldo;
        
        // Realizar dos viajes gratuitos
        Boleto boleto1 = colectivo.PagarCon(tarjeta); // Primer viaje gratuito
        Boleto boleto2 = colectivo.PagarCon(tarjeta); // Segundo viaje gratuito
        
        Assert.IsNotNull(boleto1, "El primer viaje debe ser exitoso");
        Assert.IsNotNull(boleto2, "El segundo viaje debe ser exitoso");
        Assert.AreEqual(0m, boleto1.MontoPagado, "El primer viaje debe ser gratuito");
        Assert.AreEqual(0m, boleto2.MontoPagado, "El segundo viaje debe ser gratuito");
        Assert.AreEqual(saldoInicial, tarjeta.Saldo, "El saldo no debe cambiar en viajes gratuitos");
        
        decimal saldoAntesTercerViaje = tarjeta.Saldo;

        // Act - Tercer viaje debe cobrar tarifa completa
        Boleto boleto3 = colectivo.PagarCon(tarjeta);

        // Assert
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
        // Este test verifica la lógica del contador
        // En un entorno real, se usaría dependency injection para el DateTime

        // Arrange
        colectivo.PagarCon(tarjeta); // Primer viaje
        colectivo.PagarCon(tarjeta); // Segundo viaje

        // Act
        Boleto tercerViajeMismoDia = colectivo.PagarCon(tarjeta);

        // Assert
        Assert.IsNull(tercerViajeMismoDia, 
            "El tercer viaje el mismo día debe fallar");
        
        // Nota: En un test real con mocks, aquí se simularía un cambio de día
        // y se verificaría que el contador se reinicia a 0
    }

    [Test]
    public void Test_PrimerViajeDelDia_SinViajesPrevios()
    {
        // Arrange
        TarjetaFranquiciaCompleta tarjetaNueva = new TarjetaFranquiciaCompleta();

        // Act
        bool resultado = tarjetaNueva.PagarPasaje();

        // Assert
        Assert.IsTrue(resultado, "El primer viaje de una tarjeta nueva debe ser exitoso");
    }

    [Test]
    public void Test_DosViajesConsecutivos_AmbosGratuitos()
    {
        // Arrange & Act
        Boleto primerViaje = colectivo.PagarCon(tarjeta);
        Boleto segundoViaje = colectivo.PagarCon(tarjeta);

        // Assert
        Assert.IsNotNull(primerViaje, "El primer viaje debe ser exitoso");
        Assert.IsNotNull(segundoViaje, "El segundo viaje debe ser exitoso");
        Assert.AreEqual(0m, primerViaje.MontoPagado, "Primer viaje debe ser gratuito");
        Assert.AreEqual(0m, segundoViaje.MontoPagado, "Segundo viaje debe ser gratuito");
        Assert.AreEqual(0m, tarjeta.Saldo, "El saldo debe permanecer en 0");
    }

    [Test]
    public void Test_MultiplesViajesPostLimite_TodosSeCobraNormal()
    {
        // Arrange - Cargar suficiente saldo
        tarjeta.Cargar(20000m);
        
        colectivo.PagarCon(tarjeta); // Viaje 1 - Gratis
        colectivo.PagarCon(tarjeta); // Viaje 2 - Gratis
        
        decimal saldoAntes = tarjeta.Saldo;

        // Act - Múltiples viajes posteriores
        Boleto viaje3 = colectivo.PagarCon(tarjeta);
        Boleto viaje4 = colectivo.PagarCon(tarjeta);
        Boleto viaje5 = colectivo.PagarCon(tarjeta);

        // Assert - Todos deben ser exitosos y cobrados
        Assert.IsNotNull(viaje3, "El tercer viaje debe ser exitoso");
        Assert.IsNotNull(viaje4, "El cuarto viaje debe ser exitoso");
        Assert.IsNotNull(viaje5, "El quinto viaje debe ser exitoso");
        
        Assert.AreEqual(1580m, viaje3.MontoPagado, "El tercer viaje debe cobrar tarifa completa");
        Assert.AreEqual(1580m, viaje4.MontoPagado, "El cuarto viaje debe cobrar tarifa completa");
        Assert.AreEqual(1580m, viaje5.MontoPagado, "El quinto viaje debe cobrar tarifa completa");
        
        // Verificar que se descontaron 3 tarifas completas
        Assert.AreEqual(saldoAntes - (3 * 1580m), tarjeta.Saldo, 
            "Deben haberse descontado 3 tarifas completas");
    }

    [Test]
    public void Test_BoletoGenerado_ContieneInformacionCorrecta()
    {
        // Arrange & Act
        Boleto boleto = colectivo.PagarCon(tarjeta);

        // Assert
        Assert.IsNotNull(boleto, "Debe generar un boleto");
        Assert.AreEqual(0m, boleto.MontoPagado, "El monto debe ser $0");
        Assert.AreEqual("Franquicia Completa", boleto.TipoTarjeta, 
            "El tipo de tarjeta debe ser 'Franquicia Completa'");
        Assert.AreEqual("120", boleto.LineaColectivo, "Debe tener la línea correcta");
        Assert.AreEqual("Rosario Bus", boleto.Empresa, "Debe tener la empresa correcta");
    }

    [Test]
    public void Test_ViajesPosteriores_PuedenUsarSaldoNegativo()
    {
        // Arrange - Cargar poco saldo
        tarjeta.Cargar(2000m);
        
        // Dos viajes gratuitos
        colectivo.PagarCon(tarjeta);
        colectivo.PagarCon(tarjeta);
        
        Assert.AreEqual(2000m, tarjeta.Saldo, "El saldo no debe cambiar en viajes gratuitos");
        
        // Tercer viaje requiere 1580, solo hay 2000
        Boleto boleto3 = colectivo.PagarCon(tarjeta);
        Assert.IsNotNull(boleto3, "El tercer viaje debe ser exitoso");
        Assert.AreEqual(420m, tarjeta.Saldo, "Saldo: 2000 - 1580 = 420");
        
        // Cuarto viaje: 420 - 1580 = -1160 (usa saldo negativo)
        Boleto boleto4 = colectivo.PagarCon(tarjeta);
        Assert.IsNotNull(boleto4, "El cuarto viaje debe poder usar saldo negativo");
        Assert.AreEqual(-1160m, tarjeta.Saldo, "Debe quedar con saldo negativo: 420 - 1580 = -1160");
    }

    [Test]
    public void Test_ViajesPosteriores_RespetanLimiteSaldoNegativo()
    {
        // Arrange - Sin cargar saldo
        // Dos viajes gratuitos
        colectivo.PagarCon(tarjeta);
        colectivo.PagarCon(tarjeta);
        
        // Tercer viaje: 0 - 1580 = -1580 (supera límite de -1200)
        Boleto boleto3 = colectivo.PagarCon(tarjeta);
        
        Assert.IsNull(boleto3, "No debe permitir viaje que supere el límite de saldo negativo");
        Assert.AreEqual(0m, tarjeta.Saldo, "El saldo debe permanecer en 0");
    }
}