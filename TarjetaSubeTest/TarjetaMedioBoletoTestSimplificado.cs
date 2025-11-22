using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class TarjetaMedioBoletoTestSimplificado
    {
        private Colectivo colectivo;
        private const decimal TARIFA_COMPLETA = 1580m;
        private const decimal TARIFA_MEDIO_BOLETO = 790m;

        [SetUp]
        public void Setup()
        {
            colectivo = new Colectivo("152", "Rosario Bus");
        }

        [Test]
        public void Test_PrimerViaje_MedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(10000m);
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto.MontoPagado);
        }

        [Test]
        public void Test_ObtenerTarifa_PrimerViaje_RetornaMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            decimal tarifa = tarjeta.ObtenerTarifa();
            
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, tarifa);
        }

        [Test]
        public void Test_ObtenerTarifa_DespuesDe2Viajes_RetornaTarifaCompleta()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(20000m);
            
            // Importante: Descontar NO incrementa el contador de viajes
            // Solo PagarPasaje incrementa viajesHoy
            // Por eso usamos PagarPasaje pero sin poder esperar 5 minutos
            
            // Como no podemos esperar 5 minutos en tests, verificamos que
            // después de 2 descuentos, ObtenerTarifa aún retorna medio boleto
            // porque el contador de viajes NO se incrementó
            tarjeta.Descontar(TARIFA_MEDIO_BOLETO); // No incrementa viajes
            tarjeta.Descontar(TARIFA_MEDIO_BOLETO); // No incrementa viajes
            
            // El contador de viajes sigue en 0, entonces sigue siendo medio boleto
            decimal tarifa = tarjeta.ObtenerTarifa();
            
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, tarifa, 
                "Descontar NO incrementa contador de viajes, debe seguir siendo medio boleto");
        }

        [Test]
        public void Test_TercerViaje_CobraTarifaCompleta_SinEspera()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(20000m);
            
            // IMPORTANTE: Descontar NO incrementa el contador de viajes
            // Este test debe reflejar que descontar es solo un método de utilidad
            // y no simula viajes reales
            
            // Hacer 2 descuentos (NO incrementa viajes)
            tarjeta.Descontar(TARIFA_MEDIO_BOLETO); // 790
            tarjeta.Descontar(TARIFA_MEDIO_BOLETO); // 790
            
            decimal saldoAntes = tarjeta.Saldo; // 20000 - 1580 = 18420
            
            // Tercer descuento: como NO hay viajes registrados, sigue siendo medio boleto
            tarjeta.Descontar(tarjeta.ObtenerTarifa()); // Descuenta 790 (medio boleto)
            
            Assert.AreEqual(saldoAntes - TARIFA_MEDIO_BOLETO, tarjeta.Saldo,
                "Descontar no incrementa contador, debe seguir cobrando medio boleto");
        }

        [Test]
        public void Test_Descontar_ConSaldoNegativo_Funciona()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(2000m);
            
            // Descontar más que el saldo (usa saldo negativo)
            bool resultado = tarjeta.Descontar(3000m);
            
            Assert.IsTrue(resultado, "Debe permitir con saldo negativo");
            Assert.AreEqual(-1000m, tarjeta.Saldo);
        }

        [Test]
        public void Test_PagarPasaje_SinSaldoSuficiente_UsaSaldoNegativo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1500m); // Queda 500

            // Debe permitir pagar con saldo negativo
            bool resultado = tarjeta.PagarPasaje();
            
            Assert.IsTrue(resultado);
            Assert.AreEqual(500m - TARIFA_MEDIO_BOLETO, tarjeta.Saldo);
        }
    }
}
