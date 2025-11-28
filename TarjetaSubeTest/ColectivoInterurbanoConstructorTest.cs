/*using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    /// <summary>
    /// Tests para aumentar cobertura de ColectivoInterurbano
    /// </summary>
    [TestFixture]
    public class ColectivoInterurbanoConstructorTest
    {
        [Test]
        public void Test_ConstructorSinTiempoProvider_CreaColectivoCorrectamente()
        {
            ColectivoInterurbano colectivo = new ColectivoInterurbano("500", "Galvez Express");
            
            Assert.IsNotNull(colectivo);
            Assert.AreEqual("500", colectivo.Linea);
            Assert.AreEqual("Galvez Express", colectivo.Empresa);
        }

        [Test]
        public void Test_ConstructorSinTiempoProvider_PuedeRealizarViajes()
        {
            ColectivoInterurbano colectivo = new ColectivoInterurbano("500", "Galvez Express");
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000m);
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(3000m, boleto.MontoPagado);
            Assert.AreEqual("500", boleto.LineaColectivo);
            Assert.AreEqual("Galvez Express", boleto.Empresa);
        }

        [Test]
        public void Test_ConstructorSinTiempoProvider_UsaFechaActual()
        {
            ColectivoInterurbano colectivo = new ColectivoInterurbano("501", "Baigorria Trans");
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000m);
            
            DateTime antes = DateTime.Now;
            Boleto boleto = colectivo.PagarCon(tarjeta);
            DateTime despues = DateTime.Now;
            
            Assert.GreaterOrEqual(boleto.FechaHora, antes);
            Assert.LessOrEqual(boleto.FechaHora, despues);
        }

        [Test]
        public void Test_AmbosConstructores_FuncionanCorrectamente()
        {
            // Constructor sin tiempo
            ColectivoInterurbano colectivo1 = new ColectivoInterurbano("500", "Empresa 1");
            
            // Constructor con tiempo
            TiempoSimulado tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            ColectivoInterurbano colectivo2 = new ColectivoInterurbano("501", "Empresa 2", tiempo);
            
            Assert.AreEqual("500", colectivo1.Linea);
            Assert.AreEqual("501", colectivo2.Linea);
        }
    }
}*/
