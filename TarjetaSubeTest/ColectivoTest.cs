using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class ColectivoTest
    {
        private Colectivo colectivo;

        [SetUp]
        public void Setup()
        {
            colectivo = new Colectivo("152", "Rosario Bus");
        }

        [Test]
        public void TestLinea()
        {
            Assert.AreEqual("152", colectivo.Linea);
        }

        [Test]
        public void TestEmpresa()
        {
            Assert.AreEqual("Rosario Bus", colectivo.Empresa);
        }

        [Test]
        public void TestPagarConTarjetaNula()
        {
            Boleto boleto = colectivo.PagarCon(null);
            Assert.IsNull(boleto);
        }

        [Test]
        public void TestPagarConSaldoInsuficiente_UsaSaldoNegativo()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000);
            tarjeta.Descontar(500); // Saldo: 1500

            // Con saldo negativo, permite el viaje: 1500 - 1580 = -80
            Boleto boleto = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto, "Debe permitir viaje con saldo negativo");
            Assert.AreEqual(1580m, boleto.MontoPagado);
            Assert.AreEqual(-80m, boleto.SaldoRestante, "Debe quedar con saldo negativo");
        }

        [Test]
        public void TestPagarConSaldoExacto()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000);
            tarjeta.Descontar(420);

            Boleto boleto = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto);
            Assert.AreEqual(1580m, boleto.MontoPagado);
            Assert.AreEqual(0m, boleto.SaldoRestante);
            Assert.AreEqual("152", boleto.LineaColectivo);
            Assert.AreEqual("Rosario Bus", boleto.Empresa);
        }

        [Test]
        public void TestPagarConSaldoSuficiente()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(3000);

            Boleto boleto = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto);
            Assert.AreEqual(1580m, boleto.MontoPagado);
            Assert.AreEqual(1420m, boleto.SaldoRestante); 
            Assert.AreEqual("152", boleto.LineaColectivo);
            Assert.AreEqual("Rosario Bus", boleto.Empresa);
        }

        [Test]
        public void TestPagarVariosViajes()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1);
            Assert.AreEqual(3420m, boleto1.SaldoRestante);

            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto2);
            Assert.AreEqual(1840m, boleto2.SaldoRestante);

            Boleto boleto3 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto3);
            Assert.AreEqual(260m, boleto3.SaldoRestante);
        }

        [Test]
        public void TestDatosColectivo()
        {
            Colectivo colectivoK = new Colectivo("K", "Las Delicias");
            Assert.AreEqual("K", colectivoK.Linea);
            Assert.AreEqual("Las Delicias", colectivoK.Empresa);
        }

        [Test]
        public void TestBoletoContieneInformacionDelColectivo()
        {
            Colectivo colectivo143 = new Colectivo("143", "Semtur");
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            Boleto boleto = colectivo143.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual("143", boleto.LineaColectivo);
            Assert.AreEqual("Semtur", boleto.Empresa);
        }

        [Test]
        public void TestPagarConMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(3000);

            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(790m, boleto.MontoPagado);
            Assert.AreEqual(2210m, boleto.SaldoRestante);
        }

        [Test]
        public void TestPagarConFranquiciaCompleta()
        {
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta();

            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(0m, boleto.MontoPagado);
            Assert.AreEqual(0m, boleto.SaldoRestante);
        }

        [Test]
        public void TestPagarConBoletoGratuitoEstudiantil()
        {
            TarjetaBoletoGratuitoEstudiantil tarjeta = new TarjetaBoletoGratuitoEstudiantil();

            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(0m, boleto.MontoPagado);
            Assert.AreEqual(0m, boleto.SaldoRestante);
        }

        [Test]
        public void TestMedioBoletoConSaldoInsuficiente_UsaSaldoNegativo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(2000);
            tarjeta.Descontar(1500); // Queda 500

            // Con saldo negativo, permite el viaje: 500 - 790 = -290
            Boleto boleto = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto, "Debe permitir viaje con saldo negativo");
            Assert.AreEqual(790m, boleto.MontoPagado);
            Assert.AreEqual(-290m, boleto.SaldoRestante, "Debe quedar con saldo negativo");
        }

        [Test]
        public void TestPolimorfismoConDiferentesTarjetas()
        {
            // Tarjeta normal
            Tarjeta tarjetaNormal = new Tarjeta();
            tarjetaNormal.Cargar(3000);
            Boleto boletoNormal = colectivo.PagarCon(tarjetaNormal);
            Assert.AreEqual(1580m, boletoNormal.MontoPagado);

            // Medio boleto
            TarjetaMedioBoleto medioBoleto = new TarjetaMedioBoleto();
            medioBoleto.Cargar(3000);
            Boleto boletoMedio = colectivo.PagarCon(medioBoleto);
            Assert.AreEqual(790m, boletoMedio.MontoPagado);

            // Franquicia completa
            TarjetaFranquiciaCompleta franquicia = new TarjetaFranquiciaCompleta();
            Boleto boletoGratis = colectivo.PagarCon(franquicia);
            Assert.AreEqual(0m, boletoGratis.MontoPagado);
        }

        [Test]
        public void TestFranquiciaCompletaMultiplesViajes()
        {
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta();
            tarjeta.Cargar(20000m); // Cargar saldo para viajes posteriores

            // Los primeros dos viajes deben ser exitosos y gratuitos
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1, "Viaje 1 debería generar boleto");
            Assert.AreEqual(0m, boleto1.MontoPagado, "Viaje 1 debe ser gratuito");

            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto2, "Viaje 2 debería generar boleto");
            Assert.AreEqual(0m, boleto2.MontoPagado, "Viaje 2 debe ser gratuito");

            // Del tercer viaje en adelante deben ser exitosos pero cobrados
            for (int i = 3; i <= 5; i++)
            {
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.IsNotNull(boleto, $"Viaje {i} debería generar boleto");
                Assert.AreEqual(1580m, boleto.MontoPagado, 
                    $"Viaje {i} debe cobrar tarifa completa (límite de viajes gratuitos alcanzado)");
            }
        }
    }
}