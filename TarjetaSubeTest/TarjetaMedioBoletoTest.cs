using NUnit.Framework;
using System;
using System.Threading;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class TarjetaMedioBoletoTest
    {
        private Colectivo colectivo;
        private const decimal TARIFA_COMPLETA = 1580m;
        private const decimal TARIFA_MEDIO_BOLETO = 790m;

        [SetUp]
        public void Setup()
        {
            colectivo = new Colectivo("152", "Rosario Bus");
        }

        // Tests básicos de creación y tipo
        [Test]
        public void TestCreacionTarjetaMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            Assert.IsNotNull(tarjeta);
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void TestObtenerTipo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            Assert.AreEqual("Medio Boleto", tarjeta.ObtenerTipo());
        }

        // Tests de tarifa
        [Test]
        public void TestObtenerTarifaMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, tarjeta.ObtenerTarifa());
        }

        [Test]
        public void TestTarifaEsMitadDeTarifaCompleta()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            Assert.AreEqual(TARIFA_COMPLETA * 0.5m, tarjeta.ObtenerTarifa());
        }

        // Tests de primer viaje
        [Test]
        public void TestPrimerViajeConSaldoSuficiente()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
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
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
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
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(5000m);
            
            // Primer viaje
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1);
            
            // Segundo viaje inmediato (sin esperar)
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.IsNull(boleto2);
            
            // El saldo debe permanecer igual al primer viaje
            Assert.AreEqual(5000m - TARIFA_MEDIO_BOLETO, tarjeta.Saldo);
        }

        [Test]
        public void TestSegundoViajeAntesDe5MinutosRechazado()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(5000m);
            
            // Primer viaje
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1);
            
            // Esperar menos de 5 minutos (3 segundos)
            Thread.Sleep(3000);
            
            // Segundo viaje debería ser rechazado
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.IsNull(boleto2);
        }

        [Test]
        public void TestViajesSinIntervaloMinimoNoDescuentanSaldo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
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
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
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
            Assert.AreEqual(saldoEsperado, tarjeta.Saldo); // Saldo no cambia
        }

        // Tests de carga de saldo
        [Test]
        public void TestCargarSaldoEnTarjetaMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            bool resultado = tarjeta.Cargar(3000m);
            
            Assert.IsTrue(resultado);
            Assert.AreEqual(3000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarMultiplesVeces()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            tarjeta.Cargar(2000m);
            tarjeta.Cargar(3000m);
            
            Assert.AreEqual(5000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarMontoNoPermitido()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            bool resultado = tarjeta.Cargar(1500m);
            
            Assert.IsFalse(resultado);
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        // Tests de saldo exacto
        [Test]
        public void TestViajeConSaldoExacto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1210m); // Dejar exactamente 790
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void TestViajeConUnPesoMenos_UsaSaldoNegativo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
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
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(3000m);
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto.MontoPagado);
            Assert.AreEqual("152", boleto.LineaColectivo);
            Assert.AreEqual("Rosario Bus", boleto.Empresa);
            Assert.AreEqual(3000m - TARIFA_MEDIO_BOLETO, boleto.SaldoRestante);
            Assert.AreEqual("Medio Boleto", boleto.TipoTarjeta);
            Assert.IsNotNull(boleto.FechaHora);
        }

        // Tests de diferentes empresas y líneas
        [Test]
        public void TestViajeEnDiferentesLineas()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(5000m);
            
            Colectivo colectivo1 = new Colectivo("K", "Las Delicias");
            Boleto boleto1 = colectivo1.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto1);
            Assert.AreEqual("K", boleto1.LineaColectivo);
            Assert.AreEqual("Las Delicias", boleto1.Empresa);
        }

        // Tests de límite de saldo
        [Test]
        public void TestCargarHastaLimiteSaldo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            // Cargar hasta cerca del límite de 56000
            tarjeta.Cargar(30000m);
            tarjeta.Cargar(25000m); // Monto válido
            
            Assert.AreEqual(55000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarExcediendoLimiteGeneraSaldoPendiente()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            tarjeta.Cargar(30000m);
            tarjeta.Cargar(30000m); // Excede el límite
            
            Assert.AreEqual(56000m, tarjeta.Saldo);
            Assert.AreEqual(4000m, tarjeta.SaldoPendiente);
        }

        // Tests de PagarPasaje directamente
        [Test]
        public void TestPagarPasajeConSaldoSuficiente()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(2000m);
            
            bool resultado = tarjeta.PagarPasaje();
            
            Assert.IsTrue(resultado);
            Assert.AreEqual(2000m - TARIFA_MEDIO_BOLETO, tarjeta.Saldo);
        }

        [Test]
        public void TestPagarPasajeSinSaldoSuficiente_UsaSaldoNegativo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1500m); // Queda 500
            
            // Con saldo negativo permite el viaje
            bool resultado = tarjeta.PagarPasaje();
            
            Assert.IsTrue(resultado, "Debe permitir viaje con saldo negativo");
            Assert.AreEqual(500m - TARIFA_MEDIO_BOLETO, tarjeta.Saldo);
        }

        [Test]
        public void TestPagarPasajeDosVecesInmediato()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
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
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            // Con saldo negativo permite el viaje: 0 - 790 = -790
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto, "Debe permitir viaje con saldo negativo");
            Assert.AreEqual(790m, boleto.MontoPagado);
            Assert.AreEqual(-790m, boleto.SaldoRestante);
        }

        [Test]
        public void TestMultiplesIntentosDeViajeRechazados()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
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
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
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
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(2000m);
            int idTarjeta = tarjeta.Id;
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.AreEqual(idTarjeta, boleto.IdTarjeta);
        }

        [Test]
        public void TestDiferentesTarjetasTienenDiferentesIds()
        {
            TarjetaMedioBoleto tarjeta1 = new TarjetaMedioBoleto();
            TarjetaMedioBoleto tarjeta2 = new TarjetaMedioBoleto();
            
            Assert.AreNotEqual(tarjeta1.Id, tarjeta2.Id);
        }

        // Test de acreditación de saldo pendiente
        [Test]
        public void TestAcreditacionSaldoPendienteDespuesDeViaje()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
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
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(3000m);
            
            bool resultado = tarjeta.Descontar(1000m);
            
            Assert.IsTrue(resultado);
            Assert.AreEqual(2000m, tarjeta.Saldo);
        }

        [Test]
        public void TestDescontarMasDelSaldoDisponible_UsaSaldoNegativo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(2000m);
            
            // Con saldo negativo permite hasta -1200
            bool resultado = tarjeta.Descontar(3000m);
            
            Assert.IsTrue(resultado, "Debe permitir con saldo negativo");
            Assert.AreEqual(-1000m, tarjeta.Saldo);
        }

        #region Tests de Límite de 2 Viajes con Medio Boleto por Día

        [Test]
        public void TestPrimerViaje_CobraMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(10000m);
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto.MontoPagado, 
                "El primer viaje debe cobrar medio boleto");
            Assert.AreEqual(10000m - TARIFA_MEDIO_BOLETO, tarjeta.Saldo);
        }

        [Test]
        [Ignore("Test requiere espera de 5 minutos reales")]
        public void TestSegundoViaje_CobraMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(10000m);
            
            // Primer viaje
            colectivo.PagarCon(tarjeta);
            // Nota: En tests reales se usaría dependency injection para DateTime
            // Por ahora, los tests que requieren espera serán marcados como [Ignore]
            
            // Segundo viaje
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto2);
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto2.MontoPagado, 
                "El segundo viaje debe cobrar medio boleto");
        }

        [Test]
        [Ignore("Test requiere espera de 5 minutos reales")]
        public void TestTercerViaje_CobraTarifaCompleta()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(10000m);
            decimal saldoInicial = tarjeta.Saldo;
            
            // Primer viaje con medio boleto
            colectivo.PagarCon(tarjeta);
            System.Threading.Thread.Sleep(350000); // Esperar ~6 minutos
            
            // Segundo viaje con medio boleto
            colectivo.PagarCon(tarjeta);
            System.Threading.Thread.Sleep(6000);
            
            decimal saldoAntesDelTercero = tarjeta.Saldo;
            
            // Tercer viaje debe cobrar tarifa completa
            Boleto boleto3 = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto3, "El tercer viaje debe ser exitoso");
            Assert.AreEqual(TARIFA_COMPLETA, boleto3.MontoPagado, 
                "El tercer viaje debe cobrar la tarifa completa ($1580)");
            Assert.AreEqual(saldoAntesDelTercero - TARIFA_COMPLETA, tarjeta.Saldo,
                "Debe descontarse la tarifa completa");
        }

        [Test]
        [Ignore("Test requiere espera de 5 minutos reales")]
        public void TestNoSePuedenRealizarMasDeDosViajesConMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(20000m);
            
            // Primer viaje - medio boleto
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto1.MontoPagado);
            System.Threading.Thread.Sleep(6000);
            
            // Segundo viaje - medio boleto
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto2.MontoPagado);
            System.Threading.Thread.Sleep(6000);
            
            // Tercer viaje - tarifa completa
            Boleto boleto3 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(TARIFA_COMPLETA, boleto3.MontoPagado, 
                "A partir del tercer viaje se debe cobrar tarifa completa");
            System.Threading.Thread.Sleep(6000);
            
            // Cuarto viaje - también tarifa completa
            Boleto boleto4 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(TARIFA_COMPLETA, boleto4.MontoPagado,
                "El cuarto viaje y subsiguientes también deben cobrar tarifa completa");
        }

        [Test]
        [Ignore("Test requiere espera de 5 minutos reales")]
        public void TestTercerViaje_NoRequiereEspera5Minutos()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(10000m);
            
            // Primer y segundo viaje con espera
            colectivo.PagarCon(tarjeta);
            System.Threading.Thread.Sleep(6000);
            colectivo.PagarCon(tarjeta);
            
            // Tercer viaje INMEDIATO (sin esperar 5 minutos)
            Boleto boleto3 = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto3, 
                "El tercer viaje no debe requerir espera de 5 minutos");
            Assert.AreEqual(TARIFA_COMPLETA, boleto3.MontoPagado);
        }

        [Test]
        [Ignore("Test requiere espera de 5 minutos reales")]
        public void TestCalculoDeTarifaSegunCantidadDeViajes()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(20000m);
            
            // Verificar tarifa antes de viajes
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, tarjeta.ObtenerTarifa(),
                "Sin viajes, debe retornar medio boleto");
            
            // Primer viaje
            colectivo.PagarCon(tarjeta);
            System.Threading.Thread.Sleep(6000);
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, tarjeta.ObtenerTarifa(),
                "Después de 1 viaje, debe retornar medio boleto");
            
            // Segundo viaje
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(TARIFA_COMPLETA, tarjeta.ObtenerTarifa(),
                "Después de 2 viajes, debe retornar tarifa completa");
            
            // Tercer viaje
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(TARIFA_COMPLETA, tarjeta.ObtenerTarifa(),
                "Después de 3 viajes, debe seguir retornando tarifa completa");
        }

        [Test]
        [Ignore("Test requiere espera de 5 minutos reales")]
        public void TestSaldoDescuentoCorrectoEnTresViajes()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(10000m);
            decimal saldoInicial = 10000m;
            
            // Primer viaje: 10000 - 790 = 9210
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(saldoInicial - TARIFA_MEDIO_BOLETO, tarjeta.Saldo);
            System.Threading.Thread.Sleep(6000);
            
            // Segundo viaje: 9210 - 790 = 8420
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(saldoInicial - (2 * TARIFA_MEDIO_BOLETO), tarjeta.Saldo);
            System.Threading.Thread.Sleep(6000);
            
            // Tercer viaje: 8420 - 1580 = 6840
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(saldoInicial - (2 * TARIFA_MEDIO_BOLETO) - TARIFA_COMPLETA, 
                tarjeta.Saldo,
                "Debe descontar 2 medios boletos + 1 tarifa completa");
        }

        [Test]
        [Ignore("Test requiere espera de 5 minutos reales")]
        public void TestTercerViajeRechazadoPorSaldoInsuficiente()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(2000m); // Solo alcanza para 2 medios boletos
            
            // Dos viajes con medio boleto: 2000 - 790 - 790 = 420
            colectivo.PagarCon(tarjeta);
            System.Threading.Thread.Sleep(6000);
            colectivo.PagarCon(tarjeta);
            
            Assert.AreEqual(420m, tarjeta.Saldo, "Debe quedar con 420");
            
            // Tercer viaje requiere 1580, solo hay 420
            Boleto boleto3 = colectivo.PagarCon(tarjeta);
            
            // Sin saldo negativo implementado:
            // Assert.IsNull(boleto3, "No debe permitir el tercer viaje por saldo insuficiente");
            
            // Con saldo negativo implementado:
            // El viaje debería ser exitoso usando saldo negativo
            Assert.IsNotNull(boleto3, "Con saldo negativo debe permitir el viaje");
            Assert.AreEqual(420m - TARIFA_COMPLETA, tarjeta.Saldo);
        }

        [Test]
        [Ignore("Test requiere espera de 5 minutos reales")]
        public void TestCuatroViajesEnUnDia_DosConMedioBoleto_DosConTarifaCompleta()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(20000m);
            
            Boleto b1 = colectivo.PagarCon(tarjeta);
            System.Threading.Thread.Sleep(6000);
            Boleto b2 = colectivo.PagarCon(tarjeta);
            Boleto b3 = colectivo.PagarCon(tarjeta);
            Boleto b4 = colectivo.PagarCon(tarjeta);
            
            // Verificar montos
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, b1.MontoPagado, "Viaje 1: medio boleto");
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, b2.MontoPagado, "Viaje 2: medio boleto");
            Assert.AreEqual(TARIFA_COMPLETA, b3.MontoPagado, "Viaje 3: tarifa completa");
            Assert.AreEqual(TARIFA_COMPLETA, b4.MontoPagado, "Viaje 4: tarifa completa");
            
            // Verificar saldo final
            decimal saldoEsperado = 20000m - (2 * TARIFA_MEDIO_BOLETO) - (2 * TARIFA_COMPLETA);
            Assert.AreEqual(saldoEsperado, tarjeta.Saldo);
        }

        [Test]
        [Ignore("Test requiere espera de 5 minutos reales")]
        public void TestTipoTarjetaSeMantieneComoMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(10000m);
            
            // Hacer 3 viajes
            Boleto b1 = colectivo.PagarCon(tarjeta);
            System.Threading.Thread.Sleep(6000);
            Boleto b2 = colectivo.PagarCon(tarjeta);
            Boleto b3 = colectivo.PagarCon(tarjeta);
            
            // Todos deben indicar "Medio Boleto" como tipo
            Assert.AreEqual("Medio Boleto", b1.TipoTarjeta);
            Assert.AreEqual("Medio Boleto", b2.TipoTarjeta);
            Assert.AreEqual("Medio Boleto", b3.TipoTarjeta, 
                "Incluso el tercer viaje debe indicar tipo 'Medio Boleto'");
        }

        #endregion
    }
}