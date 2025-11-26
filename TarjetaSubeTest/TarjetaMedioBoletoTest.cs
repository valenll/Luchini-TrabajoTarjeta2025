using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class TarjetaMedioBoletoTest
    {
        private TiempoSimulado tiempo;
        private Colectivo colectivo;
        private const decimal TARIFA_COMPLETA = 1580m;
        private const decimal TARIFA_MEDIO_BOLETO = 790m;

        [SetUp]
        public void Setup()
        {
            // Lunes 25 de noviembre de 2024 a las 10:00 AM (horario permitido)
            tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            colectivo = new Colectivo("152", "Rosario Bus", tiempo);
        }

        // Tests básicos de creación y tipo
        [Test]
        public void TestCreacionTarjetaMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            
            Assert.IsNotNull(tarjeta);
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void TestObtenerTipo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            
            Assert.AreEqual("Medio Boleto", tarjeta.ObtenerTipo());
        }

        // Tests de tarifa
        [Test]
        public void TestObtenerTarifaMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, tarjeta.ObtenerTarifa());
        }

        [Test]
        public void TestTarifaEsMitadDeTarifaCompleta()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            
            Assert.AreEqual(TARIFA_COMPLETA * 0.5m, tarjeta.ObtenerTarifa());
        }

        // Tests de primer viaje
        [Test]
        public void TestPrimerViajeConSaldoSuficiente()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(2000m);
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto.MontoPagado);
            Assert.AreEqual(2000m - TARIFA_MEDIO_BOLETO, boleto.SaldoRestante);
            Assert.AreEqual("Medio Boleto", boleto.TipoTarjeta);
        }

        [Test]
        public void TestPrimerViajeSinSaldoSuficiente_UsaSaldoNegativo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1500m); // Queda 500
            
            // Con saldo negativo permite el viaje: 500 - 790 = -290
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto, "Debe permitir viaje con saldo negativo");
            Assert.AreEqual(790m, boleto.MontoPagado);
            Assert.AreEqual(-290m, boleto.SaldoRestante);
        }

        // Tests de intervalo mínimo de 5 minutos
        [Test]
        public void TestSegundoViajeInmediatoRechazado()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);
            
            // Primer viaje
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1);
            
            // Segundo viaje inmediato (sin avanzar tiempo)
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.IsNull(boleto2);
            
            // El saldo debe permanecer igual al primer viaje
            Assert.AreEqual(5000m - TARIFA_MEDIO_BOLETO, tarjeta.Saldo);
        }

        [Test]
        public void TestSegundoViajeAntesDe5MinutosRechazado()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);
            
            // Primer viaje
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1);
            
            // Avanzar solo 3 minutos
            tiempo.AvanzarMinutos(3);
            
            // Segundo viaje debería ser rechazado
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.IsNull(boleto2);
        }

        [Test]
        public void TestSegundoViajeDespuesDe5MinutosAceptado()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);
            
            // Primer viaje
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1);
            
            // Avanzar 6 minutos
            tiempo.AvanzarMinutos(6);
            
            // Segundo viaje debe ser aceptado
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto2);
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto2.MontoPagado);
        }

        [Test]
        public void TestViajesSinIntervaloMinimoNoDescuentanSaldo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);
            
            // Primer viaje
            colectivo.PagarCon(tarjeta);
            decimal saldoDespuesPrimerViaje = tarjeta.Saldo;
            
            // Intentar segundo viaje inmediato (debe fallar)
            colectivo.PagarCon(tarjeta);
            
            // El saldo no debe cambiar
            Assert.AreEqual(saldoDespuesPrimerViaje, tarjeta.Saldo);
        }

        // Tests de múltiples viajes con espera correcta
        [Test]
        public void TestMultiplesViajesConIntervalosCorrectos()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(10000m);
            decimal saldoEsperado = 10000m;
            
            // Primer viaje
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            saldoEsperado -= TARIFA_MEDIO_BOLETO;
            Assert.IsNotNull(boleto1);
            Assert.AreEqual(saldoEsperado, tarjeta.Saldo);
            
            // Intentar viaje inmediato (debe fallar)
            Boleto boletoRechazado = colectivo.PagarCon(tarjeta);
            Assert.IsNull(boletoRechazado);
            Assert.AreEqual(saldoEsperado, tarjeta.Saldo);
        }

        // Tests de carga de saldo
        [Test]
        public void TestCargarSaldoEnTarjetaMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            
            bool resultado = tarjeta.Cargar(3000m);
            
            Assert.IsTrue(resultado);
            Assert.AreEqual(3000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarMultiplesVeces()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            
            tarjeta.Cargar(2000m);
            tarjeta.Cargar(3000m);
            
            Assert.AreEqual(5000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarMontoNoPermitido()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            
            bool resultado = tarjeta.Cargar(1500m);
            
            Assert.IsFalse(resultado);
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        // Tests de saldo exacto
        [Test]
        public void TestViajeConSaldoExacto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1210m); // Dejar exactamente 790
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void TestViajeConUnPesoMenos_UsaSaldoNegativo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1211m); // Dejar 789 (un peso menos)
            
            // Con saldo negativo permite el viaje: 789 - 790 = -1
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto, "Debe permitir viaje con saldo negativo");
            Assert.AreEqual(790m, boleto.MontoPagado);
            Assert.AreEqual(-1m, boleto.SaldoRestante);
        }

        // Tests de propiedades del boleto
        [Test]
        public void TestBoletoContieneInformacionCorrecta()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(3000m);
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto.MontoPagado);
            Assert.AreEqual("152", boleto.LineaColectivo);
            Assert.AreEqual("Rosario Bus", boleto.Empresa);
            Assert.AreEqual(3000m - TARIFA_MEDIO_BOLETO, boleto.SaldoRestante);
            Assert.AreEqual("Medio Boleto", boleto.TipoTarjeta);
            Assert.IsNotNull(boleto.FechaHora);
            Assert.AreEqual(new DateTime(2024, 11, 25, 10, 0, 0), boleto.FechaHora);
        }

        // Tests de diferentes empresas y líneas
        [Test]
        public void TestViajeEnDiferentesLineas()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);
            
            Colectivo colectivo1 = new Colectivo("K", "Las Delicias", tiempo);
            Boleto boleto1 = colectivo1.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto1);
            Assert.AreEqual("K", boleto1.LineaColectivo);
            Assert.AreEqual("Las Delicias", boleto1.Empresa);
        }

        // Tests de límite de saldo
        [Test]
        public void TestCargarHastaLimiteSaldo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            
            // Cargar hasta cerca del límite de 56000
            tarjeta.Cargar(30000m);
            tarjeta.Cargar(25000m);
            
            Assert.AreEqual(55000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarExcediendoLimiteGeneraSaldoPendiente()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            
            tarjeta.Cargar(30000m);
            tarjeta.Cargar(30000m); // Excede el límite
            
            Assert.AreEqual(56000m, tarjeta.Saldo);
            Assert.AreEqual(4000m, tarjeta.SaldoPendiente);
        }

        // Tests de PagarPasaje directamente
        [Test]
        public void TestPagarPasajeConSaldoSuficiente()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(2000m);
            
            bool resultado = tarjeta.PagarPasaje();
            
            Assert.IsTrue(resultado);
            Assert.AreEqual(2000m - TARIFA_MEDIO_BOLETO, tarjeta.Saldo);
        }

        [Test]
        public void TestPagarPasajeSinSaldoSuficiente_UsaSaldoNegativo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1500m); // Queda 500
            
            bool resultado = tarjeta.PagarPasaje();
            
            Assert.IsTrue(resultado, "Debe permitir viaje con saldo negativo");
            Assert.AreEqual(500m - TARIFA_MEDIO_BOLETO, tarjeta.Saldo);
        }

        [Test]
        public void TestPagarPasajeDosVecesInmediato()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);
            
            bool resultado1 = tarjeta.PagarPasaje();
            Assert.IsTrue(resultado1);
            
            bool resultado2 = tarjeta.PagarPasaje();
            Assert.IsFalse(resultado2); // Debe fallar por intervalo mínimo
        }

        // Tests de casos extremos
        [Test]
        public void TestViajeConSaldoCero_UsaSaldoNegativo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto, "Debe permitir viaje con saldo negativo");
            Assert.AreEqual(790m, boleto.MontoPagado);
            Assert.AreEqual(-790m, boleto.SaldoRestante);
        }

        [Test]
        public void TestMultiplesIntentosDeViajeRechazados()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);
            
            colectivo.PagarCon(tarjeta); // Primer viaje exitoso
            
            // Múltiples intentos inmediatos
            Assert.IsNull(colectivo.PagarCon(tarjeta));
            Assert.IsNull(colectivo.PagarCon(tarjeta));
            Assert.IsNull(colectivo.PagarCon(tarjeta));
            
            // El saldo solo debe haber descontado un viaje
            Assert.AreEqual(5000m - TARIFA_MEDIO_BOLETO, tarjeta.Saldo);
        }

        // Tests de consistencia
        [Test]
        public void TestConsistenciaDeSaldoDespuesDeVariosViajes()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(10000m);
            
            decimal saldoInicial = tarjeta.Saldo;
            
            // Primer viaje
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(saldoInicial - TARIFA_MEDIO_BOLETO, tarjeta.Saldo);
            
            // Intentos rechazados no deben cambiar el saldo
            colectivo.PagarCon(tarjeta);
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(saldoInicial - TARIFA_MEDIO_BOLETO, tarjeta.Saldo);
        }

        // Tests de ID de tarjeta
        [Test]
        public void TestIdTarjetaEnBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(2000m);
            int idTarjeta = tarjeta.Id;
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.AreEqual(idTarjeta, boleto.IdTarjeta);
        }

        [Test]
        public void TestDiferentesTarjetasTienenDiferentesIds()
        {
            TarjetaMedioBoleto tarjeta1 = new TarjetaMedioBoleto(tiempo);
            TarjetaMedioBoleto tarjeta2 = new TarjetaMedioBoleto(tiempo);
            
            Assert.AreNotEqual(tarjeta1.Id, tarjeta2.Id);
        }

        // Test de acreditación de saldo pendiente
        [Test]
        public void TestAcreditacionSaldoPendienteDespuesDeViaje()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(30000m);
            tarjeta.Cargar(30000m); // Genera saldo pendiente de 4000
            
            // Realizar un viaje (esto debería acreditar saldo pendiente)
            colectivo.PagarCon(tarjeta);
            
            // El saldo pendiente debería haberse acreditado parcialmente
            Assert.Less(tarjeta.SaldoPendiente, 4000m);
        }

        // Test de descuento directo
        [Test]
        public void TestDescontarMontoDesdeSaldo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(3000m);
            
            bool resultado = tarjeta.Descontar(1000m);
            
            Assert.IsTrue(resultado);
            Assert.AreEqual(2000m, tarjeta.Saldo);
        }

        [Test]
        public void TestDescontarMasDelSaldoDisponible_UsaSaldoNegativo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(2000m);
            
            bool resultado = tarjeta.Descontar(3000m);
            
            Assert.IsTrue(resultado, "Debe permitir con saldo negativo");
            Assert.AreEqual(-1000m, tarjeta.Saldo);
        }

        #region Tests de Límite de 2 Viajes con Medio Boleto por Día

        [Test]
        public void TestPrimerViaje_CobraMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(10000m);
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto.MontoPagado);
            Assert.AreEqual(10000m - TARIFA_MEDIO_BOLETO, tarjeta.Saldo);
        }

        [Test]
        public void TestSegundoViaje_CobraMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(10000m);
            
            // Primer viaje
            colectivo.PagarCon(tarjeta);
            
            // Avanzar 6 minutos
            tiempo.AvanzarMinutos(6);
            
            // Segundo viaje
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto2);
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto2.MontoPagado);
        }

        [Test]
        public void TestTercerViaje_CobraTarifaCompleta()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(10000m);
            decimal saldoInicial = tarjeta.Saldo;
            
            // Primer viaje
            colectivo.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(6);
            
            // Segundo viaje
            colectivo.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(6);
            
            decimal saldoAntesDelTercero = tarjeta.Saldo;
            
            // Tercer viaje debe cobrar tarifa completa
            Boleto boleto3 = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto3);
            Assert.AreEqual(TARIFA_COMPLETA, boleto3.MontoPagado);
            Assert.AreEqual(saldoAntesDelTercero - TARIFA_COMPLETA, tarjeta.Saldo);
        }

        [Test]
        public void TestNoSePuedenRealizarMasDeDosViajesConMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(20000m);
            
            // Primer viaje - medio boleto
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto1.MontoPagado);
            tiempo.AvanzarMinutos(6);
            
            // Segundo viaje - medio boleto
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto2.MontoPagado);
            tiempo.AvanzarMinutos(6);
            
            // Tercer viaje - tarifa completa
            Boleto boleto3 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(TARIFA_COMPLETA, boleto3.MontoPagado);
            tiempo.AvanzarMinutos(6);
            
            // Cuarto viaje - también tarifa completa
            Boleto boleto4 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(TARIFA_COMPLETA, boleto4.MontoPagado);
        }

        [Test]
        public void TestTercerViaje_NoRequiereEspera5Minutos()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(10000m);
            
            // Primer y segundo viaje con espera
            colectivo.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(6);
            colectivo.PagarCon(tarjeta);
            
            // Tercer viaje INMEDIATO (sin avanzar tiempo)
            Boleto boleto3 = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto3);
            Assert.AreEqual(TARIFA_COMPLETA, boleto3.MontoPagado);
        }

        [Test]
        public void TestReseteoViajesDiariosDespuesDeMedianoche()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(10000m);
            
            // Dos viajes con medio boleto
            colectivo.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(6);
            colectivo.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(6);
            
            // Tercer viaje - tarifa completa
            Boleto boleto3 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(TARIFA_COMPLETA, boleto3.MontoPagado);
            
            // Avanzar al día siguiente (horario permitido)
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 26, 10, 0, 0));
            
            // Primer viaje del nuevo día - debe ser medio boleto otra vez
            Boleto boleto4 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto4.MontoPagado);
        }

        #endregion
    }
}
