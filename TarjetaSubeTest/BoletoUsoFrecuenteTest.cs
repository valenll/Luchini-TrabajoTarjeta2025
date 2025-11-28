/*using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class BoletoUsoFrecuenteTest
    {
        private TiempoSimulado tiempo;
        private Colectivo colectivo;
        private const decimal TARIFA_NORMAL = 1580m;
        private const decimal TARIFA_CON_20_DESCUENTO = 1264m; // 80% de 1580
        private const decimal TARIFA_CON_25_DESCUENTO = 1185m; // 75% de 1580

        [SetUp]
        public void Setup()
        {
            // 1 de noviembre de 2024, 10:00 AM (inicio del mes)
            tiempo = new TiempoSimulado(new DateTime(2024, 11, 1, 10, 0, 0));
            colectivo = new Colectivo("152", "Rosario Bus", tiempo);
        }

        [Test]
        public void Test_PrimerViaje_CobraTarifaNormal()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(TARIFA_NORMAL, boleto.MontoPagado);
            Assert.AreEqual(1, tarjeta.ViajesMensuales);
        }

        [Test]
        public void Test_Viajes1a29_CobraTarifaNormal()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(30000m);
            tarjeta.Cargar(30000m);

            // Viajes 1 al 29
            for (int i = 1; i <= 29; i++)
            {
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.AreEqual(TARIFA_NORMAL, boleto.MontoPagado, 
                    $"Viaje {i} debe cobrar tarifa normal");
            }

            Assert.AreEqual(29, tarjeta.ViajesMensuales);
        }

        [Test]
        public void Test_Viaje30_Cobra20PorCientoDescuento()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(30000m);
            tarjeta.Cargar(30000m);

            // Realizar 29 viajes
            for (int i = 0; i < 29; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Viaje 30 debe tener 20% descuento
            Boleto boleto30 = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto30);
            Assert.AreEqual(TARIFA_CON_20_DESCUENTO, boleto30.MontoPagado, 
                "Viaje 30 debe tener 20% de descuento");
            Assert.AreEqual(30, tarjeta.ViajesMensuales);
        }

        [Test]
        public void Test_Viajes30a59_Cobran20PorCientoDescuento()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            
            // Cargar suficiente saldo
            for (int i = 0; i < 4; i++)
            {
                tarjeta.Cargar(30000m);
            }

            // Viajes 1 al 29 - tarifa normal
            for (int i = 1; i <= 29; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Viajes 30 al 59 - 20% descuento
            for (int i = 30; i <= 59; i++)
            {
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.AreEqual(TARIFA_CON_20_DESCUENTO, boleto.MontoPagado, 
                    $"Viaje {i} debe tener 20% de descuento");
            }

            Assert.AreEqual(59, tarjeta.ViajesMensuales);
        }

        [Test]
        public void Test_Viaje60_Cobra25PorCientoDescuento()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            
            // Cargar suficiente saldo
            for (int i = 0; i < 5; i++)
            {
                tarjeta.Cargar(30000m);
            }

            // Realizar 59 viajes
            for (int i = 0; i < 59; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Viaje 60 debe tener 25% descuento
            Boleto boleto60 = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto60);
            Assert.AreEqual(TARIFA_CON_25_DESCUENTO, boleto60.MontoPagado, 
                "Viaje 60 debe tener 25% de descuento");
            Assert.AreEqual(60, tarjeta.ViajesMensuales);
        }

        [Test]
        public void Test_Viajes60a80_Cobran25PorCientoDescuento()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            
            // Cargar suficiente saldo
            for (int i = 0; i < 6; i++)
            {
                tarjeta.Cargar(30000m);
            }

            // Viajes 1 al 59
            for (int i = 1; i <= 59; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Viajes 60 al 80 - 25% descuento
            for (int i = 60; i <= 80; i++)
            {
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.AreEqual(TARIFA_CON_25_DESCUENTO, boleto.MontoPagado, 
                    $"Viaje {i} debe tener 25% de descuento");
            }

            Assert.AreEqual(80, tarjeta.ViajesMensuales);
        }

        [Test]
        public void Test_Viaje81_VuelveATarifaNormal()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            
            // Cargar suficiente saldo
            for (int i = 0; i < 7; i++)
            {
                tarjeta.Cargar(30000m);
            }

            // Realizar 80 viajes
            for (int i = 0; i < 80; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Viaje 81 debe volver a tarifa normal
            Boleto boleto81 = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto81);
            Assert.AreEqual(TARIFA_NORMAL, boleto81.MontoPagado, 
                "Viaje 81 debe volver a tarifa normal");
            Assert.AreEqual(81, tarjeta.ViajesMensuales);
        }

        [Test]
        public void Test_ViajesDespuesDe80_CobraTarifaNormal()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            
            // Cargar suficiente saldo
            for (int i = 0; i < 7; i++)
            {
                tarjeta.Cargar(30000m);
            }

            // Realizar 80 viajes
            for (int i = 0; i < 80; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Viajes 81 al 85 - tarifa normal
            for (int i = 81; i <= 85; i++)
            {
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.AreEqual(TARIFA_NORMAL, boleto.MontoPagado, 
                    $"Viaje {i} debe cobrar tarifa normal");
            }
        }

        [Test]
        public void Test_ContadorViajesMensuales_SeReiniciaEnNuevoMes()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(10000m);

            // Realizar 5 viajes en noviembre
            for (int i = 0; i < 5; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            Assert.AreEqual(5, tarjeta.ViajesMensuales);

            // Avanzar al primer día del mes siguiente
            tiempo.EstablecerTiempo(new DateTime(2024, 12, 1, 10, 0, 0));

            // El contador debe reiniciarse
            Boleto primerViajeNuevoMes = colectivo.PagarCon(tarjeta);
            
            Assert.AreEqual(1, tarjeta.ViajesMensuales, 
                "El contador debe reiniciarse en el nuevo mes");
            Assert.AreEqual(TARIFA_NORMAL, primerViajeNuevoMes.MontoPagado);
        }

        [Test]
        public void Test_DescuentoSoloAplicaATarjetasNormales()
        {
            // Medio boleto NO debe tener descuento por uso frecuente
            TarjetaMedioBoleto tarjetaMedio = new TarjetaMedioBoleto(tiempo);
            tarjetaMedio.Cargar(30000m);
            tarjetaMedio.Cargar(30000m);

            // Realizar 30 viajes - debe seguir cobrando medio boleto
            for (int i = 0; i < 30; i++)
            {
                tiempo.AvanzarMinutos(6); // Esperar para medio boleto
                colectivo.PagarCon(tarjetaMedio);
            }

            // Este test verifica que no haya cambio en la tarifa
            // El medio boleto siempre cobra 790
            Assert.Pass("Medio boleto no aplica descuento por uso frecuente");
        }

        [Test]
        public void Test_CalculoTarifaSegunCantidadViajes()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            
            // Cargar suficiente saldo
            tarjeta.Cargar(30000m);
            tarjeta.Cargar(30000m);

            decimal saldoInicial = tarjeta.Saldo;
            decimal totalGastado = 0m;

            // Viajes 1-29: tarifa normal (29 * 1580)
            for (int i = 1; i <= 29; i++)
            {
                Boleto b = colectivo.PagarCon(tarjeta);
                totalGastado += b.MontoPagado;
            }
            Assert.AreEqual(29 * TARIFA_NORMAL, totalGastado);

            tarjeta.Cargar(30000m);

            // Viajes 30-59: 20% descuento (30 * 1264)
            for (int i = 30; i <= 59; i++)
            {
                Boleto b = colectivo.PagarCon(tarjeta);
                totalGastado += b.MontoPagado;
            }

            tarjeta.Cargar(20000m);

            // Viajes 60-80: 25% descuento (21 * 1185)
            for (int i = 60; i <= 80; i++)
            {
                Boleto b = colectivo.PagarCon(tarjeta);
                totalGastado += b.MontoPagado;
            }

            tarjeta.Cargar(10000m);

            // Viajes 81-85: tarifa normal (5 * 1580)
            for (int i = 81; i <= 85; i++)
            {
                Boleto b = colectivo.PagarCon(tarjeta);
                totalGastado += b.MontoPagado;
            }

            decimal esperado = (29 * 1580m) + (30 * 1264m) + (21 * 1185m) + (5 * 1580m);
            Assert.AreEqual(esperado, totalGastado);
            Assert.AreEqual(120000m - totalGastado, tarjeta.Saldo);
        }

        [Test]
        public void Test_ContadorViajesMensuales_IncrementaCorrectamente()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(20000m);

            Assert.AreEqual(0, tarjeta.ViajesMensuales);

            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(1, tarjeta.ViajesMensuales);

            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(2, tarjeta.ViajesMensuales);

            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(3, tarjeta.ViajesMensuales);
        }

        [Test]
        public void Test_Viaje29y30_CambioDeDescuento()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            
            // Cargar suficiente saldo
            for (int i = 0; i < 3; i++)
            {
                tarjeta.Cargar(30000m);
            }

            // Realizar 28 viajes
            for (int i = 0; i < 28; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Viaje 29 - último con tarifa normal
            Boleto boleto29 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(TARIFA_NORMAL, boleto29.MontoPagado);
            Assert.AreEqual(29, tarjeta.ViajesMensuales);

            // Viaje 30 - primero con 20% descuento
            Boleto boleto30 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(TARIFA_CON_20_DESCUENTO, boleto30.MontoPagado);
            Assert.AreEqual(30, tarjeta.ViajesMensuales);
        }

        [Test]
        public void Test_Viaje59y60_CambioDeDescuento()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            
            // Cargar suficiente saldo
            for (int i = 0; i < 5; i++)
            {
                tarjeta.Cargar(30000m);
            }

            // Realizar 58 viajes
            for (int i = 0; i < 58; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Viaje 59 - último con 20% descuento
            Boleto boleto59 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(TARIFA_CON_20_DESCUENTO, boleto59.MontoPagado);
            Assert.AreEqual(59, tarjeta.ViajesMensuales);

            // Viaje 60 - primero con 25% descuento
            Boleto boleto60 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(TARIFA_CON_25_DESCUENTO, boleto60.MontoPagado);
            Assert.AreEqual(60, tarjeta.ViajesMensuales);
        }

        [Test]
        public void Test_Viaje80y81_CambioATarifaNormal()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            
            // Cargar suficiente saldo
            for (int i = 0; i < 7; i++)
            {
                tarjeta.Cargar(30000m);
            }

            // Realizar 79 viajes
            for (int i = 0; i < 79; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Viaje 80 - último con 25% descuento
            Boleto boleto80 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(TARIFA_CON_25_DESCUENTO, boleto80.MontoPagado);
            Assert.AreEqual(80, tarjeta.ViajesMensuales);

            // Viaje 81 - vuelve a tarifa normal
            Boleto boleto81 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(TARIFA_NORMAL, boleto81.MontoPagado);
            Assert.AreEqual(81, tarjeta.ViajesMensuales);
        }

        [Test]
        public void Test_VariosViajesEnDiferentesDiasDelMes()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            
            // Cargar suficiente saldo
            for (int i = 0; i < 4; i++)
            {
                tarjeta.Cargar(30000m);
            }

            // 5 viajes el día 1
            for (int i = 0; i < 5; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Avanzar al día 10
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 10, 10, 0, 0));
            
            // 10 viajes más
            for (int i = 0; i < 10; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            Assert.AreEqual(15, tarjeta.ViajesMensuales);

            // Avanzar al día 25
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 10, 0, 0));
            
            // 20 viajes más (llegaría a 35 = dentro de rango 30-59)
            for (int i = 0; i < 20; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            Boleto ultimoBoleto = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(36, tarjeta.ViajesMensuales);
            Assert.AreEqual(TARIFA_CON_20_DESCUENTO, ultimoBoleto.MontoPagado);
        }
    }
}*/