/*using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class BoletoCompletaTest
    {
        private TiempoSimulado tiempo;

        [SetUp]
        public void Setup()
        {
            // Lunes 25 nov 2024, 10:00 AM
            tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
        }

        [Test]
        public void TestConstructorInicializaPropiedades()
        {
            decimal montoPagado = 1580m;
            string linea = "152";
            string empresa = "Rosario Bus";
            decimal saldoRestante = 420m;
            string tipoTarjeta = "Normal";
            int idTarjeta = 1;

            Boleto boleto = new Boleto(montoPagado, linea, empresa, saldoRestante, tipoTarjeta, idTarjeta, false, tiempo);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(montoPagado, boleto.MontoPagado);
            Assert.AreEqual(linea, boleto.LineaColectivo);
            Assert.AreEqual(empresa, boleto.Empresa);
            Assert.AreEqual(saldoRestante, boleto.SaldoRestante);
            Assert.IsNotNull(boleto.FechaHora);
        }

        [Test]
        public void TestMontoPagadoCero()
        {
            Boleto boleto = new Boleto(0m, "K", "Las Delicias", 5000m, "Franquicia Completa", 1, false, tiempo);
            Assert.AreEqual(0m, boleto.MontoPagado);
        }

        [Test]
        public void TestMontoPagadoTarifaNormal()
        {
            Boleto boleto = new Boleto(1580m, "143", "Semtur", 3420m, "Normal", 1, false, tiempo);
            Assert.AreEqual(1580m, boleto.MontoPagado);
        }

        [Test]
        public void TestMontoPagadoMedioBoleto()
        {
            Boleto boleto = new Boleto(790m, "152", "Rosario Bus", 4210m, "Medio Boleto", 1, false, tiempo);
            Assert.AreEqual(790m, boleto.MontoPagado);
        }

        [Test]
        public void TestMontoPagadoAlto()
        {
            Boleto boleto = new Boleto(9999m, "K", "Las Delicias", 100m, "Normal", 2, false, tiempo);
            Assert.AreEqual(9999m, boleto.MontoPagado);
        }

        [Test]
        public void TestLineaColectivoNumerica()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 1, false, tiempo);
            Assert.AreEqual("152", boleto.LineaColectivo);
        }

        [Test]
        public void TestLineaColectivoLetra()
        {
            Boleto boleto = new Boleto(1580m, "K", "Las Delicias", 3420m, "Normal", 1, false, tiempo);
            Assert.AreEqual("K", boleto.LineaColectivo);
        }

        [Test]
        public void TestLineaColectivoAlphanumerica()
        {
            Boleto boleto = new Boleto(1580m, "143K", "Semtur", 2000m, "Normal", 1, false, tiempo);
            Assert.AreEqual("143K", boleto.LineaColectivo);
        }

        [Test]
        public void TestLineaColectivoVacia()
        {
            Boleto boleto = new Boleto(1580m, "", "Rosario Bus", 420m, "Normal", 1, false, tiempo);
            Assert.AreEqual("", boleto.LineaColectivo);
        }

        [Test]
        public void TestEmpresaRosarioBus()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 1, false, tiempo);
            Assert.AreEqual("Rosario Bus", boleto.Empresa);
        }

        [Test]
        public void TestEmpresaSemtur()
        {
            Boleto boleto = new Boleto(2000m, "143", "Semtur", 3000m, "Normal", 1, false, tiempo);
            Assert.AreEqual("Semtur", boleto.Empresa);
        }

        [Test]
        public void TestEmpresaLasDelicias()
        {
            Boleto boleto = new Boleto(790m, "K", "Las Delicias", 4210m, "Medio Boleto", 1, false, tiempo);
            Assert.AreEqual("Las Delicias", boleto.Empresa);
        }

        [Test]
        public void TestEmpresaVacia()
        {
            Boleto boleto = new Boleto(1580m, "152", "", 420m, "Normal", 1, false, tiempo);
            Assert.AreEqual("", boleto.Empresa);
        }

        [Test]
        public void TestSaldoRestanteCero()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 0m, "Normal", 1, false, tiempo);
            Assert.AreEqual(0m, boleto.SaldoRestante);
        }

        [Test]
        public void TestSaldoRestanteBajo()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 1, false, tiempo);
            Assert.AreEqual(420m, boleto.SaldoRestante);
        }

        [Test]
        public void TestSaldoRestanteMedio()
        {
            Boleto boleto = new Boleto(1580m, "K", "Las Delicias", 2500m, "Normal", 1, false, tiempo);
            Assert.AreEqual(2500m, boleto.SaldoRestante);
        }

        [Test]
        public void TestSaldoRestanteAlto()
        {
            Boleto boleto = new Boleto(1580m, "143", "Semtur", 54420m, "Normal", 1, false, tiempo);
            Assert.AreEqual(54420m, boleto.SaldoRestante);
        }

        [Test]
        public void TestSaldoRestanteMaximo()
        {
            Boleto boleto = new Boleto(0m, "K", "Las Delicias", 56000m, "Franquicia Completa", 1, false, tiempo);
            Assert.AreEqual(56000m, boleto.SaldoRestante);
        }

        [Test]
        public void TestTipoTarjetaNormal()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 1, false, tiempo);
            Assert.AreEqual("Normal", boleto.TipoTarjeta);
        }

        [Test]
        public void TestTipoTarjetaMedioBoleto()
        {
            Boleto boleto = new Boleto(790m, "K", "Las Delicias", 4210m, "Medio Boleto", 1, false, tiempo);
            Assert.AreEqual("Medio Boleto", boleto.TipoTarjeta);
        }

        [Test]
        public void TestTipoTarjetaFranquiciaCompleta()
        {
            Boleto boleto = new Boleto(0m, "143", "Semtur", 5000m, "Franquicia Completa", 1, false, tiempo);
            Assert.AreEqual("Franquicia Completa", boleto.TipoTarjeta);
        }

        [Test]
        public void TestTipoTarjetaBoletoGratuitoEstudiantil()
        {
            // TarjetaBoletoGratuitoEstudiantil hereda de TarjetaFranquiciaCompleta, 
            // por lo que devuelve "Franquicia Completa"
            Boleto boleto = new Boleto(0m, "152", "Rosario Bus", 3000m, "Franquicia Completa", 2, false, tiempo);
            Assert.AreEqual("Franquicia Completa", boleto.TipoTarjeta);
        }

        [Test]
        public void TestIdTarjetaUno()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 1, false, tiempo);
            Assert.AreEqual(1, boleto.IdTarjeta);
        }

        [Test]
        public void TestIdTarjetaDos()
        {
            Boleto boleto = new Boleto(790m, "K", "Las Delicias", 4210m, "Medio Boleto", 2, false, tiempo);
            Assert.AreEqual(2, boleto.IdTarjeta);
        }

        [Test]
        public void TestIdTarjetaAlto()
        {
            Boleto boleto = new Boleto(0m, "143", "Semtur", 5000m, "Franquicia Completa", 999, false, tiempo);
            Assert.AreEqual(999, boleto.IdTarjeta);
        }

        [Test]
        public void TestIdTarjetaCero()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 0, false, tiempo);
            Assert.AreEqual(0, boleto.IdTarjeta);
        }

        [Test]
        public void TestFechaHoraNoNull()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 1, false, tiempo);
            Assert.IsNotNull(boleto.FechaHora);
        }

        [Test]
        public void TestFechaHoraConTiempoSimulado()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 1, false, tiempo);
            Assert.AreEqual(new DateTime(2024, 11, 25, 10, 0, 0), boleto.FechaHora);
        }

        [Test]
        public void TestFechaHoraDiferentesInstancias()
        {
            Boleto boleto1 = new Boleto(1580m, "152", "Rosario Bus", 3420m, "Normal", 1, false, tiempo);
            Assert.AreEqual(new DateTime(2024, 11, 25, 10, 0, 0), boleto1.FechaHora);

            // Avanzar tiempo
            tiempo.AvanzarMinutos(30);
            Boleto boleto2 = new Boleto(1580m, "K", "Las Delicias", 1840m, "Normal", 1, false, tiempo);
            Assert.AreEqual(new DateTime(2024, 11, 25, 10, 30, 0), boleto2.FechaHora);

            Assert.LessOrEqual(boleto1.FechaHora, boleto2.FechaHora);
        }

        [Test]
        public void TestBoletoCompleto1()
        {
            decimal montoPagado = 1580m;
            string linea = "152";
            string empresa = "Rosario Bus";
            decimal saldoRestante = 420m;
            string tipoTarjeta = "Normal";
            int idTarjeta = 1;

            Boleto boleto = new Boleto(montoPagado, linea, empresa, saldoRestante, tipoTarjeta, idTarjeta, false, tiempo);

            Assert.AreEqual(montoPagado, boleto.MontoPagado);
            Assert.AreEqual(linea, boleto.LineaColectivo);
            Assert.AreEqual(empresa, boleto.Empresa);
            Assert.AreEqual(saldoRestante, boleto.SaldoRestante);
            Assert.AreEqual(tipoTarjeta, boleto.TipoTarjeta);
            Assert.AreEqual(idTarjeta, boleto.IdTarjeta);
            Assert.IsNotNull(boleto.FechaHora);
            Assert.AreEqual(new DateTime(2024, 11, 25, 10, 0, 0), boleto.FechaHora);
        }

        [Test]
        public void TestBoletoCompleto2()
        {
            Boleto boleto = new Boleto(790m, "K", "Las Delicias", 4210m, "Medio Boleto", 2, false, tiempo);

            Assert.AreEqual(790m, boleto.MontoPagado);
            Assert.AreEqual("K", boleto.LineaColectivo);
            Assert.AreEqual("Las Delicias", boleto.Empresa);
            Assert.AreEqual(4210m, boleto.SaldoRestante);
            Assert.AreEqual("Medio Boleto", boleto.TipoTarjeta);
            Assert.AreEqual(2, boleto.IdTarjeta);
            Assert.IsNotNull(boleto.FechaHora);
        }

        [Test]
        public void TestBoletoCompleto3()
        {
            Boleto boleto = new Boleto(0m, "143", "Semtur", 5000m, "Franquicia Completa", 3, false, tiempo);

            Assert.AreEqual(0m, boleto.MontoPagado);
            Assert.AreEqual("143", boleto.LineaColectivo);
            Assert.AreEqual("Semtur", boleto.Empresa);
            Assert.AreEqual(5000m, boleto.SaldoRestante);
            Assert.AreEqual("Franquicia Completa", boleto.TipoTarjeta);
            Assert.AreEqual(3, boleto.IdTarjeta);
            Assert.IsNotNull(boleto.FechaHora);
        }

        [Test]
        public void TestMultiplesBoletosIndependientes()
        {
            Boleto boleto1 = new Boleto(1580m, "152", "Rosario Bus", 3420m, "Normal", 1, false, tiempo);
            
            tiempo.AvanzarMinutos(10);
            Boleto boleto2 = new Boleto(790m, "K", "Las Delicias", 1840m, "Medio Boleto", 2, false, tiempo);
            
            tiempo.AvanzarMinutos(10);
            Boleto boleto3 = new Boleto(0m, "143", "Semtur", 2630m, "Franquicia Completa", 3, false, tiempo);

            // Verificar boleto 1
            Assert.AreEqual(1580m, boleto1.MontoPagado);
            Assert.AreEqual("152", boleto1.LineaColectivo);
            Assert.AreEqual("Rosario Bus", boleto1.Empresa);
            Assert.AreEqual(3420m, boleto1.SaldoRestante);
            Assert.AreEqual("Normal", boleto1.TipoTarjeta);
            Assert.AreEqual(1, boleto1.IdTarjeta);
            Assert.AreEqual(new DateTime(2024, 11, 25, 10, 0, 0), boleto1.FechaHora);

            // Verificar boleto 2
            Assert.AreEqual(790m, boleto2.MontoPagado);
            Assert.AreEqual("K", boleto2.LineaColectivo);
            Assert.AreEqual("Las Delicias", boleto2.Empresa);
            Assert.AreEqual(1840m, boleto2.SaldoRestante);
            Assert.AreEqual("Medio Boleto", boleto2.TipoTarjeta);
            Assert.AreEqual(2, boleto2.IdTarjeta);
            Assert.AreEqual(new DateTime(2024, 11, 25, 10, 10, 0), boleto2.FechaHora);

            // Verificar boleto 3
            Assert.AreEqual(0m, boleto3.MontoPagado);
            Assert.AreEqual("143", boleto3.LineaColectivo);
            Assert.AreEqual("Semtur", boleto3.Empresa);
            Assert.AreEqual(2630m, boleto3.SaldoRestante);
            Assert.AreEqual("Franquicia Completa", boleto3.TipoTarjeta);
            Assert.AreEqual(3, boleto3.IdTarjeta);
            Assert.AreEqual(new DateTime(2024, 11, 25, 10, 20, 0), boleto3.FechaHora);
        }

        [Test]
        public void TestValoresExtremos()
        {
            Boleto boleto = new Boleto(decimal.MaxValue, "999", "Empresa Test", decimal.MaxValue, "Normal", int.MaxValue, false, tiempo);

            Assert.AreEqual(decimal.MaxValue, boleto.MontoPagado);
            Assert.AreEqual("999", boleto.LineaColectivo);
            Assert.AreEqual("Empresa Test", boleto.Empresa);
            Assert.AreEqual(decimal.MaxValue, boleto.SaldoRestante);
            Assert.AreEqual("Normal", boleto.TipoTarjeta);
            Assert.AreEqual(int.MaxValue, boleto.IdTarjeta);
        }

        [Test]
        public void TestPropiedadesInmutables()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 1, false, tiempo);

            decimal montoPagadoOriginal = boleto.MontoPagado;
            string lineaOriginal = boleto.LineaColectivo;
            string empresaOriginal = boleto.Empresa;
            decimal saldoOriginal = boleto.SaldoRestante;
            string tipoOriginal = boleto.TipoTarjeta;
            int idOriginal = boleto.IdTarjeta;
            DateTime fechaOriginal = boleto.FechaHora;

            // Avanzar tiempo
            tiempo.AvanzarMinutos(10);

            // Las propiedades deben permanecer iguales
            Assert.AreEqual(montoPagadoOriginal, boleto.MontoPagado);
            Assert.AreEqual(lineaOriginal, boleto.LineaColectivo);
            Assert.AreEqual(empresaOriginal, boleto.Empresa);
            Assert.AreEqual(saldoOriginal, boleto.SaldoRestante);
            Assert.AreEqual(tipoOriginal, boleto.TipoTarjeta);
            Assert.AreEqual(idOriginal, boleto.IdTarjeta);
            Assert.AreEqual(fechaOriginal, boleto.FechaHora);
        }

        [Test]
        public void TestBoletosDeTiposDeTarjeta()
        {
            // Simular boleto de tarjeta normal
            Boleto boletoNormal = new Boleto(1580m, "152", "Rosario Bus", 3420m, "Normal", 1, false, tiempo);
            Assert.AreEqual(1580m, boletoNormal.MontoPagado);
            Assert.AreEqual("Normal", boletoNormal.TipoTarjeta);

            // Simular boleto de medio boleto
            Boleto boletoMedio = new Boleto(790m, "K", "Las Delicias", 4210m, "Medio Boleto", 2, false, tiempo);
            Assert.AreEqual(790m, boletoMedio.MontoPagado);
            Assert.AreEqual("Medio Boleto", boletoMedio.TipoTarjeta);

            // Simular boleto gratuito
            Boleto boletoGratuito = new Boleto(0m, "143", "Semtur", 5000m, "Franquicia Completa", 3, false, tiempo);
            Assert.AreEqual(0m, boletoGratuito.MontoPagado);
            Assert.AreEqual("Franquicia Completa", boletoGratuito.TipoTarjeta);

            // Simular boleto gratuito estudiantil (también usa "Franquicia Completa")
            Boleto boletoEstudiantil = new Boleto(0m, "152", "Rosario Bus", 3000m, "Franquicia Completa", 4, false, tiempo);
            Assert.AreEqual(0m, boletoEstudiantil.MontoPagado);
            Assert.AreEqual("Franquicia Completa", boletoEstudiantil.TipoTarjeta);
        }

        [Test]
        public void TestLineaConEspacios()
        {
            Boleto boleto = new Boleto(1580m, "152 Sur", "Rosario Bus", 420m, "Normal", 1, false, tiempo);
            Assert.AreEqual("152 Sur", boleto.LineaColectivo);
        }

        [Test]
        public void TestEmpresaConCaracteresEspeciales()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus S.A.", 420m, "Normal", 1, false, tiempo);
            Assert.AreEqual("Rosario Bus S.A.", boleto.Empresa);
        }

        [Test]
        public void TestSaldoRestanteDecimales()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420.50m, "Normal", 1, false, tiempo);
            Assert.AreEqual(420.50m, boleto.SaldoRestante);
        }

        [Test]
        public void TestMontoPagadoDecimales()
        {
            Boleto boleto = new Boleto(1580.75m, "152", "Rosario Bus", 420m, "Normal", 1, false, tiempo);
            Assert.AreEqual(1580.75m, boleto.MontoPagado);
        }

        [Test]
        public void TestDiferentesIdsTarjeta()
        {
            Boleto boleto1 = new Boleto(1580m, "152", "Rosario Bus", 3420m, "Normal", 1, false, tiempo);
            Boleto boleto2 = new Boleto(1580m, "152", "Rosario Bus", 1840m, "Normal", 2, false, tiempo);
            Boleto boleto3 = new Boleto(1580m, "152", "Rosario Bus", 260m, "Normal", 3, false, tiempo);

            Assert.AreEqual(1, boleto1.IdTarjeta);
            Assert.AreEqual(2, boleto2.IdTarjeta);
            Assert.AreEqual(3, boleto3.IdTarjeta);
        }

        [Test]
        public void TestTodosLosTiposDeTarjeta()
        {
            Boleto boletoNormal = new Boleto(1580m, "152", "Rosario Bus", 3420m, "Normal", 1, false, tiempo);
            Boleto boletoMedio = new Boleto(790m, "K", "Las Delicias", 4210m, "Medio Boleto", 2, false, tiempo);
            Boleto boletoFranquicia = new Boleto(0m, "143", "Semtur", 5000m, "Franquicia Completa", 3, false, tiempo);
            Boleto boletoEstudiantil = new Boleto(0m, "152", "Rosario Bus", 3000m, "Franquicia Completa", 4, false, tiempo);

            Assert.AreEqual("Normal", boletoNormal.TipoTarjeta);
            Assert.AreEqual("Medio Boleto", boletoMedio.TipoTarjeta);
            Assert.AreEqual("Franquicia Completa", boletoFranquicia.TipoTarjeta);
            Assert.AreEqual("Franquicia Completa", boletoEstudiantil.TipoTarjeta);
        }

        [Test]
        public void TestTodasLasPropiedadesNoNulas()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 1, false, tiempo);

            Assert.IsNotNull(boleto.MontoPagado);
            Assert.IsNotNull(boleto.LineaColectivo);
            Assert.IsNotNull(boleto.Empresa);
            Assert.IsNotNull(boleto.SaldoRestante);
            Assert.IsNotNull(boleto.TipoTarjeta);
            Assert.IsNotNull(boleto.IdTarjeta);
            Assert.IsNotNull(boleto.FechaHora);
        }

        [Test]
        public void TestBoletoTrasbordo()
        {
            // Test de boleto marcado como trasbordo
            Boleto boletoTrasbordo = new Boleto(0m, "152", "Rosario Bus", 3420m, "Normal", 1, true, tiempo);
            Assert.IsTrue(boletoTrasbordo.EsTrasbordo);
            Assert.AreEqual(0m, boletoTrasbordo.MontoPagado);

            // Test de boleto normal (no trasbordo)
            Boleto boletoNormal = new Boleto(1580m, "K", "Las Delicias", 1840m, "Normal", 1, false, tiempo);
            Assert.IsFalse(boletoNormal.EsTrasbordo);
            Assert.AreEqual(1580m, boletoNormal.MontoPagado);
        }
    }
}*/
