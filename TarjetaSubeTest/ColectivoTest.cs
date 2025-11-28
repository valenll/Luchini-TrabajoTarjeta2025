/*using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class ColectivoTest
    {
        private TiempoSimulado tiempo;
        private Colectivo colectivo;

        [SetUp]
        public void Setup()
        {
            // Lunes 25 nov 2024, 10:00 AM (horario permitido para franquicias)
            tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            colectivo = new Colectivo("152", "Rosario Bus", tiempo);
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
        public void Test_PagarConSaldoInsuficiente_UsaSaldoNegativo()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
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
            Tarjeta tarjeta = new Tarjeta(tiempo);
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
            Tarjeta tarjeta = new Tarjeta(tiempo);
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
            Tarjeta tarjeta = new Tarjeta(tiempo);
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
            Colectivo colectivoK = new Colectivo("K", "Las Delicias", tiempo);
            Assert.AreEqual("K", colectivoK.Linea);
            Assert.AreEqual("Las Delicias", colectivoK.Empresa);
        }

        [Test]
        public void TestBoletoContieneInformacionDelColectivo()
        {
            Colectivo colectivo143 = new Colectivo("143", "Semtur", tiempo);
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000);

            Boleto boleto = colectivo143.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual("143", boleto.LineaColectivo);
            Assert.AreEqual("Semtur", boleto.Empresa);
        }

        [Test]
        public void TestPagarConMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(3000);

            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(790m, boleto.MontoPagado);
            Assert.AreEqual(2210m, boleto.SaldoRestante);
        }

        [Test]
        public void TestPagarConFranquiciaCompleta()
        {
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta(tiempo);

            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(0m, boleto.MontoPagado);
            Assert.AreEqual(0m, boleto.SaldoRestante);
        }

        [Test]
        public void TestPagarConBoletoGratuitoEstudiantil()
        {
            TarjetaBoletoGratuitoEstudiantil tarjeta = new TarjetaBoletoGratuitoEstudiantil(tiempo);

            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(0m, boleto.MontoPagado);
            Assert.AreEqual(0m, boleto.SaldoRestante);
        }

        [Test]
        public void TestMedioBoletoConSaldoInsuficiente_UsaSaldoNegativo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
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
            Tarjeta tarjetaNormal = new Tarjeta(tiempo);
            tarjetaNormal.Cargar(3000);
            Boleto boletoNormal = colectivo.PagarCon(tarjetaNormal);
            Assert.AreEqual(1580m, boletoNormal.MontoPagado);

            // Medio boleto
            TarjetaMedioBoleto medioBoleto = new TarjetaMedioBoleto(tiempo);
            medioBoleto.Cargar(3000);
            Boleto boletoMedio = colectivo.PagarCon(medioBoleto);
            Assert.AreEqual(790m, boletoMedio.MontoPagado);

            // Franquicia completa
            TarjetaFranquiciaCompleta franquicia = new TarjetaFranquiciaCompleta(tiempo);
            Boleto boletoGratis = colectivo.PagarCon(franquicia);
            Assert.AreEqual(0m, boletoGratis.MontoPagado);
        }

        [Test]
        public void TestFranquiciaCompletaMultiplesViajes()
        {
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta(tiempo);
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

        [Test]
        public void TestTarjetaNormalFuncionaCualquierHorario()
        {
            // Probar sábado
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 23, 10, 0, 0));
            Tarjeta tarjeta1 = new Tarjeta(tiempo);
            tarjeta1.Cargar(5000);
            Boleto boleto1 = colectivo.PagarCon(tarjeta1);
            Assert.IsNotNull(boleto1, "Tarjeta normal funciona en sábado");

            // Probar domingo madrugada
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 24, 3, 0, 0));
            Tarjeta tarjeta2 = new Tarjeta(tiempo);
            tarjeta2.Cargar(5000);
            Boleto boleto2 = colectivo.PagarCon(tarjeta2);
            Assert.IsNotNull(boleto2, "Tarjeta normal funciona domingo de madrugada");

            // Probar lunes noche
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 23, 0, 0));
            Tarjeta tarjeta3 = new Tarjeta(tiempo);
            tarjeta3.Cargar(5000);
            Boleto boleto3 = colectivo.PagarCon(tarjeta3);
            Assert.IsNotNull(boleto3, "Tarjeta normal funciona lunes de noche");
        }

        [Test]
        public void TestFranquiciasFueraDeHorario_NoFuncionan()
        {
            // Cambiar a sábado (fuera de L-V)
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 23, 10, 0, 0));

            TarjetaMedioBoleto medio = new TarjetaMedioBoleto(tiempo);
            medio.Cargar(5000);
            Boleto boletoMedio = colectivo.PagarCon(medio);
            Assert.IsNull(boletoMedio, "Medio boleto no funciona en sábado");

            TarjetaFranquiciaCompleta franquicia = new TarjetaFranquiciaCompleta(tiempo);
            Boleto boletoFranquicia = colectivo.PagarCon(franquicia);
            Assert.IsNull(boletoFranquicia, "Franquicia completa no funciona en sábado");
        }

        [Test]
        public void TestBoletoTieneFechaCorrecta()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(new DateTime(2024, 11, 25, 10, 0, 0), boleto.FechaHora);
        }

        [Test]
        public void TestMultiplesBoletosDiferentesFechas()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(10000);

            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(new DateTime(2024, 11, 25, 10, 0, 0), boleto1.FechaHora);

            // Avanzar 2 horas
            tiempo.AvanzarHoras(2);
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(new DateTime(2024, 11, 25, 12, 0, 0), boleto2.FechaHora);

            // Avanzar al día siguiente
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 26, 10, 0, 0));
            Boleto boleto3 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(new DateTime(2024, 11, 26, 10, 0, 0), boleto3.FechaHora);
        }
    }
}*/
