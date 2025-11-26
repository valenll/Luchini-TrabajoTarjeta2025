using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    /// <summary>
    /// Tests simplificados para TarjetaMedioBoleto usando TiempoSimulado.
    /// Estos tests son más rápidos y confiables que usar Thread.Sleep.
    /// </summary>
    [TestFixture]
    public class TarjetaMedioBoletoTestSimplificado
    {
        private TiempoSimulado tiempo;
        private Colectivo colectivo;
        private const decimal TARIFA_COMPLETA = 1580m;
        private const decimal TARIFA_MEDIO_BOLETO = 790m;

        [SetUp]
        public void Setup()
        {
            // Lunes 25 nov 2024, 10:00 AM (horario permitido)
            tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            colectivo = new Colectivo("152", "Rosario Bus", tiempo);
        }

        [Test]
        public void Test_PrimerViaje_MedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(10000m);
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto.MontoPagado);
        }

        [Test]
        public void Test_ObtenerTarifa_PrimerViaje_RetornaMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            
            decimal tarifa = tarjeta.ObtenerTarifa();
            
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, tarifa);
        }

        [Test]
        public void Test_ObtenerTarifa_DespuesDe2Viajes_RetornaTarifaCompleta()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(20000m);
            
            // Realizar dos viajes con medio boleto
            colectivo.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(6);
            colectivo.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(6);
            
            // El tercero debe ser tarifa completa
            decimal tarifa = tarjeta.ObtenerTarifa();
            
            Assert.AreEqual(TARIFA_COMPLETA, tarifa);
        }

        [Test]
        public void Test_SegundoViaje_MedioBoleto_DespuesDe5Minutos()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(10000m);
            
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto1);
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto1.MontoPagado);
            
            // Avanzar más de 5 minutos
            tiempo.AvanzarMinutos(6);
            
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto2);
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto2.MontoPagado);
        }

        [Test]
        public void Test_TercerViaje_TarifaCompleta()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(10000m);
            
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(6);
            Assert.AreEqual(TARIFA_COMPLETA * 0.5m, boleto1.MontoPagado);
            
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(6);
            Assert.AreEqual(TARIFA_COMPLETA * 0.5m, boleto2.MontoPagado);
            
            Boleto boleto3 = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto3);
            Assert.AreEqual(TARIFA_COMPLETA, boleto3.MontoPagado);
            Assert.AreEqual(10000m - (TARIFA_MEDIO_BOLETO * 2m + TARIFA_COMPLETA), tarjeta.Saldo);
        }

        [Test]
        public void Test_PagarPasaje_ConSaldoSuficiente()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);
            
            bool resultado = tarjeta.PagarPasaje();
            
            Assert.IsTrue(resultado);
            Assert.AreEqual(5000m - TARIFA_MEDIO_BOLETO, tarjeta.Saldo);
        }

        [Test]
        public void Test_PagarPasaje_SinSaldoSuficiente_UsaSaldoNegativo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(2000m);
            
            Boleto boleto1 = colectivo.PagarCon(tarjeta); // Primer viaje: 2000 - 790 = 1210
            Assert.IsNotNull(boleto1, "Debe permitir primer viaje");
            Assert.AreEqual(790m, boleto1.MontoPagado);
            Assert.AreEqual(1210m, tarjeta.Saldo);
            
            tiempo.AvanzarMinutos(15);
            
            Boleto boleto2 = colectivo.PagarCon(tarjeta); // 2do viaje: 1210 - 790 = 420
            Assert.IsNotNull(boleto2, "Debe permitir segundo viaje");
            Assert.AreEqual(790m, boleto2.MontoPagado);
            Assert.AreEqual(420m, tarjeta.Saldo);
            
            tiempo.AvanzarMinutos(15);
            
            Boleto boleto3 = colectivo.PagarCon(tarjeta); // 3er viaje: 420 - 1580 = -1160
            
            Assert.IsNotNull(boleto3, "Debe permitir tercer viaje con saldo negativo");
            Assert.AreEqual(1580m, boleto3.MontoPagado);
            Assert.AreEqual(-1160m, tarjeta.Saldo);
            Assert.IsTrue(tarjeta.Saldo < 0, "El saldo debe ser negativo");
        }

        [Test]
        public void Test_SegundoViajeAntesDe5Minutos_Rechazado()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);
            
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1);
            
            // Intentar viajar antes de 5 minutos
            tiempo.AvanzarMinutos(3);
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            
            Assert.IsNull(boleto2, "No debe permitir viajar antes de 5 minutos");
        }

        [Test]
        public void Test_FueraDeHorario_Rechazado()
        {
            // Cambiar a sábado (fuera de L-V)
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 23, 10, 0, 0));
            
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNull(boleto, "No debe permitir viajar en sábado");
        }

        [Test]
        public void Test_HoraNoche_Rechazado()
        {
            // Cambiar a lunes pero a las 23:00 (fuera de 6-22)
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 23, 0, 0));
            
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNull(boleto, "No debe permitir viajar fuera de horario 6-22");
        }

        [Test]
        public void Test_NuevoDia_ReiniciaContadorViajes()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(10000m);
            
            // Hacer 3 viajes en un día
            colectivo.PagarCon(tarjeta); // Medio boleto
            tiempo.AvanzarMinutos(6);
            colectivo.PagarCon(tarjeta); // Medio boleto
            tiempo.AvanzarMinutos(6);
            Boleto boleto3 = colectivo.PagarCon(tarjeta); // Tarifa completa
            Assert.AreEqual(TARIFA_COMPLETA, boleto3.MontoPagado);
            
            // Avanzar al día siguiente
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 26, 10, 0, 0)); // Martes
            
            // Primer viaje del nuevo día debe ser medio boleto otra vez
            Boleto boleto4 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto4.MontoPagado);
        }
    }
}
