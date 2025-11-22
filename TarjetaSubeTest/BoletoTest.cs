using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class BoletoCompletaTest
    {
        [Test]
        public void TestConstructorInicializaPropiedades()
        {
            decimal montoPagado = 1580m;
            string linea = "152";
            string empresa = "Rosario Bus";
            decimal saldoRestante = 420m;
            string tipoTarjeta = "Normal";
            int idTarjeta = 1;

            Boleto boleto = new Boleto(montoPagado, linea, empresa, saldoRestante, tipoTarjeta, idTarjeta);

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
            Boleto boleto = new Boleto(0m, "K", "Las Delicias", 5000m, "FranquiciaCompleta", 1);
            Assert.AreEqual(0m, boleto.MontoPagado);
        }

        [Test]
        public void TestMontoPagadoTarifaNormal()
        {
            Boleto boleto = new Boleto(1580m, "143", "Semtur", 3420m, "Normal", 1);
            Assert.AreEqual(1580m, boleto.MontoPagado);
        }

        [Test]
        public void TestMontoPagadoMedioBoleto()
        {
            Boleto boleto = new Boleto(790m, "152", "Rosario Bus", 4210m, "MedioBoleto", 1);
            Assert.AreEqual(790m, boleto.MontoPagado);
        }

        [Test]
        public void TestMontoPagadoAlto()
        {
            Boleto boleto = new Boleto(9999m, "K", "Las Delicias", 100m, "Normal", 2);
            Assert.AreEqual(9999m, boleto.MontoPagado);
        }

        [Test]
        public void TestLineaColectivoNumerica()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 1);
            Assert.AreEqual("152", boleto.LineaColectivo);
        }

        [Test]
        public void TestLineaColectivoLetra()
        {
            Boleto boleto = new Boleto(1580m, "K", "Las Delicias", 3420m, "Normal", 1);
            Assert.AreEqual("K", boleto.LineaColectivo);
        }

        [Test]
        public void TestLineaColectivoAlphanumerica()
        {
            Boleto boleto = new Boleto(1580m, "143K", "Semtur", 2000m, "Normal", 1);
            Assert.AreEqual("143K", boleto.LineaColectivo);
        }

        [Test]
        public void TestLineaColectivoVacia()
        {
            Boleto boleto = new Boleto(1580m, "", "Rosario Bus", 420m, "Normal", 1);
            Assert.AreEqual("", boleto.LineaColectivo);
        }

        [Test]
        public void TestEmpresaRosarioBus()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 1);
            Assert.AreEqual("Rosario Bus", boleto.Empresa);
        }

        [Test]
        public void TestEmpresaSemtur()
        {
            Boleto boleto = new Boleto(2000m, "143", "Semtur", 3000m, "Normal", 1);
            Assert.AreEqual("Semtur", boleto.Empresa);
        }

        [Test]
        public void TestEmpresaLasDelicias()
        {
            Boleto boleto = new Boleto(790m, "K", "Las Delicias", 4210m, "MedioBoleto", 1);
            Assert.AreEqual("Las Delicias", boleto.Empresa);
        }

        [Test]
        public void TestEmpresaVacia()
        {
            Boleto boleto = new Boleto(1580m, "152", "", 420m, "Normal", 1);
            Assert.AreEqual("", boleto.Empresa);
        }

        [Test]
        public void TestSaldoRestanteCero()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 0m, "Normal", 1);
            Assert.AreEqual(0m, boleto.SaldoRestante);
        }

        [Test]
        public void TestSaldoRestanteBajo()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 1);
            Assert.AreEqual(420m, boleto.SaldoRestante);
        }

        [Test]
        public void TestSaldoRestanteMedio()
        {
            Boleto boleto = new Boleto(1580m, "K", "Las Delicias", 2500m, "Normal", 1);
            Assert.AreEqual(2500m, boleto.SaldoRestante);
        }

        [Test]
        public void TestSaldoRestanteAlto()
        {
            Boleto boleto = new Boleto(1580m, "143", "Semtur", 54420m, "Normal", 1);
            Assert.AreEqual(54420m, boleto.SaldoRestante);
        }

        [Test]
        public void TestSaldoRestanteMaximo()
        {
            Boleto boleto = new Boleto(0m, "K", "Las Delicias", 56000m, "FranquiciaCompleta", 1);
            Assert.AreEqual(56000m, boleto.SaldoRestante);
        }

        [Test]
        public void TestTipoTarjetaNormal()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 1);
            Assert.AreEqual("Normal", boleto.TipoTarjeta);
        }

        [Test]
        public void TestTipoTarjetaMedioBoleto()
        {
            Boleto boleto = new Boleto(790m, "K", "Las Delicias", 4210m, "MedioBoleto", 1);
            Assert.AreEqual("MedioBoleto", boleto.TipoTarjeta);
        }

        [Test]
        public void TestTipoTarjetaFranquiciaCompleta()
        {
            Boleto boleto = new Boleto(0m, "143", "Semtur", 5000m, "FranquiciaCompleta", 1);
            Assert.AreEqual("FranquiciaCompleta", boleto.TipoTarjeta);
        }

        [Test]
        public void TestTipoTarjetaBoletoGratuitoEstudiantil()
        {
            Boleto boleto = new Boleto(0m, "152", "Rosario Bus", 3000m, "BoletoGratuitoEstudiantil", 2);
            Assert.AreEqual("BoletoGratuitoEstudiantil", boleto.TipoTarjeta);
        }

        [Test]
        public void TestIdTarjetaUno()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 1);
            Assert.AreEqual(1, boleto.IdTarjeta);
        }

        [Test]
        public void TestIdTarjetaDos()
        {
            Boleto boleto = new Boleto(790m, "K", "Las Delicias", 4210m, "MedioBoleto", 2);
            Assert.AreEqual(2, boleto.IdTarjeta);
        }

        [Test]
        public void TestIdTarjetaAlto()
        {
            Boleto boleto = new Boleto(0m, "143", "Semtur", 5000m, "FranquiciaCompleta", 999);
            Assert.AreEqual(999, boleto.IdTarjeta);
        }

        [Test]
        public void TestIdTarjetaCero()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 0);
            Assert.AreEqual(0, boleto.IdTarjeta);
        }

        [Test]
        public void TestFechaHoraNoNull()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 1);
            Assert.IsNotNull(boleto.FechaHora);
        }

        [Test]
        public void TestFechaHoraActual()
        {
            DateTime antes = DateTime.Now;
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 1);
            DateTime despues = DateTime.Now;

            Assert.GreaterOrEqual(boleto.FechaHora, antes);
            Assert.LessOrEqual(boleto.FechaHora, despues);
        }

        [Test]
        public void TestFechaHoraDiferentesInstancias()
        {
            Boleto boleto1 = new Boleto(1580m, "152", "Rosario Bus", 3420m, "Normal", 1);
            System.Threading.Thread.Sleep(10);
            Boleto boleto2 = new Boleto(1580m, "K", "Las Delicias", 1840m, "Normal", 1);

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

            Boleto boleto = new Boleto(montoPagado, linea, empresa, saldoRestante, tipoTarjeta, idTarjeta);

            Assert.AreEqual(montoPagado, boleto.MontoPagado);
            Assert.AreEqual(linea, boleto.LineaColectivo);
            Assert.AreEqual(empresa, boleto.Empresa);
            Assert.AreEqual(saldoRestante, boleto.SaldoRestante);
            Assert.AreEqual(tipoTarjeta, boleto.TipoTarjeta);
            Assert.AreEqual(idTarjeta, boleto.IdTarjeta);
            Assert.IsNotNull(boleto.FechaHora);
        }

        [Test]
        public void TestBoletoCompleto2()
        {
            Boleto boleto = new Boleto(790m, "K", "Las Delicias", 4210m, "MedioBoleto", 2);

            Assert.AreEqual(790m, boleto.MontoPagado);
            Assert.AreEqual("K", boleto.LineaColectivo);
            Assert.AreEqual("Las Delicias", boleto.Empresa);
            Assert.AreEqual(4210m, boleto.SaldoRestante);
            Assert.AreEqual("MedioBoleto", boleto.TipoTarjeta);
            Assert.AreEqual(2, boleto.IdTarjeta);
            Assert.IsNotNull(boleto.FechaHora);
        }

        [Test]
        public void TestBoletoCompleto3()
        {
            Boleto boleto = new Boleto(0m, "143", "Semtur", 5000m, "FranquiciaCompleta", 3);

            Assert.AreEqual(0m, boleto.MontoPagado);
            Assert.AreEqual("143", boleto.LineaColectivo);
            Assert.AreEqual("Semtur", boleto.Empresa);
            Assert.AreEqual(5000m, boleto.SaldoRestante);
            Assert.AreEqual("FranquiciaCompleta", boleto.TipoTarjeta);
            Assert.AreEqual(3, boleto.IdTarjeta);
            Assert.IsNotNull(boleto.FechaHora);
        }

        [Test]
        public void TestMultiplesBoletosIndependientes()
        {
            Boleto boleto1 = new Boleto(1580m, "152", "Rosario Bus", 3420m, "Normal", 1);
            Boleto boleto2 = new Boleto(790m, "K", "Las Delicias", 1840m, "MedioBoleto", 2);
            Boleto boleto3 = new Boleto(0m, "143", "Semtur", 2630m, "FranquiciaCompleta", 3);

            // Verificar boleto 1
            Assert.AreEqual(1580m, boleto1.MontoPagado);
            Assert.AreEqual("152", boleto1.LineaColectivo);
            Assert.AreEqual("Rosario Bus", boleto1.Empresa);
            Assert.AreEqual(3420m, boleto1.SaldoRestante);
            Assert.AreEqual("Normal", boleto1.TipoTarjeta);
            Assert.AreEqual(1, boleto1.IdTarjeta);

            // Verificar boleto 2
            Assert.AreEqual(790m, boleto2.MontoPagado);
            Assert.AreEqual("K", boleto2.LineaColectivo);
            Assert.AreEqual("Las Delicias", boleto2.Empresa);
            Assert.AreEqual(1840m, boleto2.SaldoRestante);
            Assert.AreEqual("MedioBoleto", boleto2.TipoTarjeta);
            Assert.AreEqual(2, boleto2.IdTarjeta);

            // Verificar boleto 3
            Assert.AreEqual(0m, boleto3.MontoPagado);
            Assert.AreEqual("143", boleto3.LineaColectivo);
            Assert.AreEqual("Semtur", boleto3.Empresa);
            Assert.AreEqual(2630m, boleto3.SaldoRestante);
            Assert.AreEqual("FranquiciaCompleta", boleto3.TipoTarjeta);
            Assert.AreEqual(3, boleto3.IdTarjeta);
        }

        [Test]
        public void TestValoresExtremos()
        {
            Boleto boleto = new Boleto(decimal.MaxValue, "999", "Empresa Test", decimal.MaxValue, "Normal", int.MaxValue);

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
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 1);

            decimal montoPagadoOriginal = boleto.MontoPagado;
            string lineaOriginal = boleto.LineaColectivo;
            string empresaOriginal = boleto.Empresa;
            decimal saldoOriginal = boleto.SaldoRestante;
            string tipoOriginal = boleto.TipoTarjeta;
            int idOriginal = boleto.IdTarjeta;
            DateTime fechaOriginal = boleto.FechaHora;

            // Simular uso del boleto
            System.Threading.Thread.Sleep(10);

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
            Boleto boletoNormal = new Boleto(1580m, "152", "Rosario Bus", 3420m, "Normal", 1);
            Assert.AreEqual(1580m, boletoNormal.MontoPagado);
            Assert.AreEqual("Normal", boletoNormal.TipoTarjeta);

            // Simular boleto de medio boleto
            Boleto boletoMedio = new Boleto(790m, "K", "Las Delicias", 4210m, "MedioBoleto", 2);
            Assert.AreEqual(790m, boletoMedio.MontoPagado);
            Assert.AreEqual("MedioBoleto", boletoMedio.TipoTarjeta);

            // Simular boleto gratuito
            Boleto boletoGratuito = new Boleto(0m, "143", "Semtur", 5000m, "FranquiciaCompleta", 3);
            Assert.AreEqual(0m, boletoGratuito.MontoPagado);
            Assert.AreEqual("FranquiciaCompleta", boletoGratuito.TipoTarjeta);

            // Simular boleto gratuito estudiantil
            Boleto boletoEstudiantil = new Boleto(0m, "152", "Rosario Bus", 3000m, "BoletoGratuitoEstudiantil", 4);
            Assert.AreEqual(0m, boletoEstudiantil.MontoPagado);
            Assert.AreEqual("BoletoGratuitoEstudiantil", boletoEstudiantil.TipoTarjeta);
        }

        [Test]
        public void TestLineaConEspacios()
        {
            Boleto boleto = new Boleto(1580m, "152 Sur", "Rosario Bus", 420m, "Normal", 1);
            Assert.AreEqual("152 Sur", boleto.LineaColectivo);
        }

        [Test]
        public void TestEmpresaConCaracteresEspeciales()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus S.A.", 420m, "Normal", 1);
            Assert.AreEqual("Rosario Bus S.A.", boleto.Empresa);
        }

        [Test]
        public void TestSaldoRestanteDecimales()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420.50m, "Normal", 1);
            Assert.AreEqual(420.50m, boleto.SaldoRestante);
        }

        [Test]
        public void TestMontoPagadoDecimales()
        {
            Boleto boleto = new Boleto(1580.75m, "152", "Rosario Bus", 420m, "Normal", 1);
            Assert.AreEqual(1580.75m, boleto.MontoPagado);
        }

        [Test]
        public void TestDiferentesIdsTarjeta()
        {
            Boleto boleto1 = new Boleto(1580m, "152", "Rosario Bus", 3420m, "Normal", 1);
            Boleto boleto2 = new Boleto(1580m, "152", "Rosario Bus", 1840m, "Normal", 2);
            Boleto boleto3 = new Boleto(1580m, "152", "Rosario Bus", 260m, "Normal", 3);

            Assert.AreEqual(1, boleto1.IdTarjeta);
            Assert.AreEqual(2, boleto2.IdTarjeta);
            Assert.AreEqual(3, boleto3.IdTarjeta);
        }

        [Test]
        public void TestTodosLosTiposDeTarjeta()
        {
            Boleto boletoNormal = new Boleto(1580m, "152", "Rosario Bus", 3420m, "Normal", 1);
            Boleto boletoMedio = new Boleto(790m, "K", "Las Delicias", 4210m, "MedioBoleto", 2);
            Boleto boletoFranquicia = new Boleto(0m, "143", "Semtur", 5000m, "FranquiciaCompleta", 3);
            Boleto boletoEstudiantil = new Boleto(0m, "152", "Rosario Bus", 3000m, "BoletoGratuitoEstudiantil", 4);

            Assert.AreEqual("Normal", boletoNormal.TipoTarjeta);
            Assert.AreEqual("MedioBoleto", boletoMedio.TipoTarjeta);
            Assert.AreEqual("FranquiciaCompleta", boletoFranquicia.TipoTarjeta);
            Assert.AreEqual("BoletoGratuitoEstudiantil", boletoEstudiantil.TipoTarjeta);
        }

        [Test]
        public void TestTodasLasPropiedadesNoNulas()
        {
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 420m, "Normal", 1);

            Assert.IsNotNull(boleto.MontoPagado);
            Assert.IsNotNull(boleto.LineaColectivo);
            Assert.IsNotNull(boleto.Empresa);
            Assert.IsNotNull(boleto.SaldoRestante);
            Assert.IsNotNull(boleto.TipoTarjeta);
            Assert.IsNotNull(boleto.IdTarjeta);
            Assert.IsNotNull(boleto.FechaHora);
        }
    }
}