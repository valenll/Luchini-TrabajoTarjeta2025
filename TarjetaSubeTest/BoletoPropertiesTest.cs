using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    /// <summary>
    /// Tests para aumentar cobertura de propiedades y métodos de Boleto
    /// </summary>
    [TestFixture]
    public class BoletoPropertiesTest
    {
        private TiempoSimulado tiempo;

        [SetUp]
        public void Setup()
        {
            tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 14, 30, 45));
        }

        [Test]
        public void Test_PropiedadFecha_DevuelveSoloLaFecha()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 3420m, "Normal", 1, false, tiempo);
            
            Assert.AreEqual(new DateTime(2024, 11, 25), boleto.Fecha);
            Assert.AreEqual(boleto.FechaHora.Date, boleto.Fecha);
        }

        [Test]
        public void Test_PropiedadTotalAbonado_EsIgualAMontoPagado()
        {
            Boleto boleto = new Boleto(790m, "K", "Las Delicias", 2000m, "Medio Boleto", 2, false, tiempo);
            
            Assert.AreEqual(boleto.MontoPagado, boleto.TotalAbonado);
            Assert.AreEqual(790m, boleto.TotalAbonado);
        }

        [Test]
        public void Test_PropiedadSaldo_EsIgualASaldoRestante()
        {
            Boleto boleto = new Boleto(1580m, "143", "Semtur", 5000m, "Normal", 3, false, tiempo);
            
            Assert.AreEqual(boleto.SaldoRestante, boleto.Saldo);
            Assert.AreEqual(5000m, boleto.Saldo);
        }

        [Test]
        public void Test_ToString_BoletoNormal()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 3420m, "Normal", 1, false, tiempo);
            
            string resultado = boleto.ToString();
            
            Assert.IsNotNull(resultado);
            Assert.That(resultado, Does.Contain("Línea: 152"));
            Assert.That(resultado, Does.Contain("Rosario Bus"));
            Assert.That(resultado, Does.Contain("Normal"));
            Assert.That(resultado, Does.Contain("$1580"));
            Assert.That(resultado, Does.Contain("$3420"));
            Assert.That(resultado, Does.Not.Contain("TRASBORDO"));
        }

        [Test]
        public void Test_ToString_BoletoTrasbordo()
        {
            Boleto boleto = new Boleto(0m, "K", "Las Delicias", 5000m, "Normal", 2, true, tiempo);
            
            string resultado = boleto.ToString();
            
            Assert.IsNotNull(resultado);
            Assert.That(resultado, Does.Contain("TRASBORDO"));
            Assert.That(resultado, Does.Contain("Línea: K"));
            Assert.That(resultado, Does.Contain("$0"));
        }

        [Test]
        public void Test_ConstructorSinTiempoProvider_UsaTiempoReal()
        {
            DateTime antes = DateTime.Now;
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 3420m, "Normal", 1, false);
            DateTime despues = DateTime.Now;
            
            Assert.GreaterOrEqual(boleto.FechaHora, antes);
            Assert.LessOrEqual(boleto.FechaHora, despues);
        }

        [Test]
        public void Test_ConstructorSinParametroTrasbordo_DefaultFalse()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 3420m, "Normal", 1);
            
            Assert.IsFalse(boleto.EsTrasbordo);
        }

        [Test]
        public void Test_ToString_ContenidoCompleto()
        {
            Boleto boleto = new Boleto(790m, "143", "Semtur", 2500m, "Medio Boleto", 5, false, tiempo);
            
            string resultado = boleto.ToString();
            
            // Verificar que contiene todos los elementos principales
            Assert.That(resultado, Does.Contain("25/11/2024"));
            Assert.That(resultado, Does.Contain("14:30"));
            Assert.That(resultado, Does.Contain("143"));
            Assert.That(resultado, Does.Contain("Semtur"));
            Assert.That(resultado, Does.Contain("Medio Boleto"));
            Assert.That(resultado, Does.Contain("ID: 5"));
        }
    }
}
