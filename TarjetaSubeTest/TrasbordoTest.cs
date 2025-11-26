using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class TrasbordoTest
    {
        private TiempoSimulado tiempo;
        private Colectivo colectivo152;
        private Colectivo colectivoK;
        private Colectivo colectivo143;
        private ColectivoInterurbano colectivo500;

        [SetUp]
        public void Setup()
        {
            // Lunes 25 nov 2024, 10:00 AM (horario válido para trasbordos: L-S 7-22)
            tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            colectivo152 = new Colectivo("152", "Rosario Bus", tiempo);
            colectivoK = new Colectivo("K", "Las Delicias", tiempo);
            colectivo143 = new Colectivo("143", "Semtur", tiempo);
            colectivo500 = new ColectivoInterurbano("500", "Galvez Express", tiempo);
        }

        #region Tests Básicos de Trasbordo

        [Test]
        public void Test_PrimerViaje_NoEsTrasbordo()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto = colectivo152.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.IsFalse(boleto.EsTrasbordo, "El primer viaje no debe ser trasbordo");
            Assert.AreEqual(1580m, boleto.MontoPagado, "Debe cobrar tarifa normal");
        }

        [Test]
        public void Test_SegundoViajeLineaDiferente_DentroDe1Hora_EsTrasbordo()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto1 = colectivo152.PagarCon(tarjeta);
            decimal saldoDespuesPrimero = tarjeta.Saldo;
            
            // Avanzar 30 minutos (menos de 1 hora)
            tiempo.AvanzarMinutos(30);
            
            Boleto boleto2 = colectivoK.PagarCon(tarjeta);

            Assert.IsNotNull(boleto1, "Primer viaje debe ser exitoso");
            Assert.IsNotNull(boleto2, "Segundo viaje debe ser exitoso");
            Assert.IsFalse(boleto1.EsTrasbordo, "Primer viaje no es trasbordo");
            Assert.IsTrue(boleto2.EsTrasbordo, "Segundo viaje debe ser trasbordo");
            Assert.AreEqual(0m, boleto2.MontoPagado, "Trasbordo no debe cobrar");
            Assert.AreEqual(saldoDespuesPrimero, tarjeta.Saldo, 
                "El saldo no debe cambiar en trasbordo");
        }

        [Test]
        public void Test_SegundoViajeMismaLinea_NoPuedeTrasbordar()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto1 = colectivo152.PagarCon(tarjeta);
            decimal saldoDespuesPrimero = tarjeta.Saldo;
            
            tiempo.AvanzarMinutos(10);
            
            Boleto boleto2 = colectivo152.PagarCon(tarjeta); // Misma línea

            Assert.IsNotNull(boleto1);
            Assert.IsNotNull(boleto2);
            Assert.IsFalse(boleto2.EsTrasbordo, 
                "No debe ser trasbordo si es la misma línea");
            Assert.AreEqual(1580m, boleto2.MontoPagado, 
                "Debe cobrar tarifa normal");
            Assert.Less(tarjeta.Saldo, saldoDespuesPrimero, 
                "El saldo debe disminuir");
        }

        [Test]
        public void Test_Trasbordo_DespuesDe1Hora_CobraNormal()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto1 = colectivo152.PagarCon(tarjeta);
            decimal saldoDespuesPrimero = tarjeta.Saldo;
            
            // Avanzar 61 minutos (más de 1 hora)
            tiempo.AvanzarMinutos(61);
            
            Boleto boleto2 = colectivoK.PagarCon(tarjeta);

            Assert.IsNotNull(boleto1);
            Assert.IsNotNull(boleto2);
            Assert.IsFalse(boleto2.EsTrasbordo, 
                "No debe ser trasbordo después de 1 hora");
            Assert.AreEqual(1580m, boleto2.MontoPagado);
            Assert.Less(tarjeta.Saldo, saldoDespuesPrimero);
        }

        [Test]
        public void Test_Trasbordo_Exactamente1Hora_NoEsTrasbordo()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            colectivo152.PagarCon(tarjeta);
            
            // Avanzar exactamente 60 minutos
            tiempo.AvanzarMinutos(60);
            
            Boleto boleto2 = colectivoK.PagarCon(tarjeta);

            Assert.IsFalse(boleto2.EsTrasbordo, 
                "A la hora exacta ya no es trasbordo");
            Assert.AreEqual(1580m, boleto2.MontoPagado);
        }

        [Test]
        public void Test_Trasbordo_59Minutos_EsTrasbordo()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            colectivo152.PagarCon(tarjeta);
            
            // Avanzar 59 minutos
            tiempo.AvanzarMinutos(59);
            
            Boleto boleto2 = colectivoK.PagarCon(tarjeta);

            Assert.IsTrue(boleto2.EsTrasbordo, 
                "Dentro de la hora debe ser trasbordo");
            Assert.AreEqual(0m, boleto2.MontoPagado);
        }

        #endregion

        #region Tests de Horario de Trasbordo

        [Test]
        public void Test_Trasbordo_DomingoDentroDeHorario_NoPermite()
        {
            // Domingo 24 nov 2024, 10:00 AM
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 24, 10, 0, 0));
            
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            colectivo152.PagarCon(tarjeta);
            decimal saldoDespuesPrimero = tarjeta.Saldo;
            
            tiempo.AvanzarMinutos(10);
            
            Boleto boleto2 = colectivoK.PagarCon(tarjeta);

            Assert.IsNotNull(boleto2);
            Assert.IsFalse(boleto2.EsTrasbordo, 
                "No debe ser trasbordo en domingo");
            Assert.AreEqual(1580m, boleto2.MontoPagado);
            Assert.Less(tarjeta.Saldo, saldoDespuesPrimero, 
                "Debe cobrar tarifa normal");
        }

        [Test]
        public void Test_Trasbordo_SabadoDentroDeHorario_Permite()
        {
            // Sábado 23 nov 2024, 10:00 AM
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 23, 10, 0, 0));
            
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            colectivo152.PagarCon(tarjeta);
            decimal saldoDespuesPrimero = tarjeta.Saldo;
            
            tiempo.AvanzarMinutos(10);
            
            Boleto boleto2 = colectivoK.PagarCon(tarjeta);

            Assert.IsNotNull(boleto2);
            Assert.IsTrue(boleto2.EsTrasbordo, 
                "Debe ser trasbordo en sábado dentro de horario");
            Assert.AreEqual(0m, boleto2.MontoPagado);
            Assert.AreEqual(saldoDespuesPrimero, tarjeta.Saldo);
        }

        [Test]
        public void Test_Trasbordo_LunesAntesDe7AM_NoPermite()
        {
            // Lunes 25 nov 2024, 6:30 AM (antes de las 7)
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 6, 30, 0));
            
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            colectivo152.PagarCon(tarjeta);
            decimal saldoDespuesPrimero = tarjeta.Saldo;
            
            tiempo.AvanzarMinutos(10);
            
            Boleto boleto2 = colectivoK.PagarCon(tarjeta);

            Assert.IsNotNull(boleto2);
            Assert.IsFalse(boleto2.EsTrasbordo, 
                "No debe ser trasbordo antes de las 7:00");
            Assert.Less(tarjeta.Saldo, saldoDespuesPrimero);
        }

        [Test]
        public void Test_Trasbordo_LunesA7AM_Permite()
        {
            // Lunes 25 nov 2024, 7:00 AM
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 7, 0, 0));
            
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            colectivo152.PagarCon(tarjeta);
            decimal saldoDespuesPrimero = tarjeta.Saldo;
            
            tiempo.AvanzarMinutos(10);
            
            Boleto boleto2 = colectivoK.PagarCon(tarjeta);

            Assert.IsTrue(boleto2.EsTrasbordo, 
                "Debe permitir trasbordo desde las 7:00");
            Assert.AreEqual(saldoDespuesPrimero, tarjeta.Saldo);
        }

        [Test]
        public void Test_Trasbordo_LunesA22PM_NoPermite()
        {
            // Lunes 25 nov 2024, 22:00
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 22, 0, 0));
            
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            colectivo152.PagarCon(tarjeta);
            decimal saldoDespuesPrimero = tarjeta.Saldo;
            
            tiempo.AvanzarMinutos(10);
            
            Boleto boleto2 = colectivoK.PagarCon(tarjeta);

            Assert.IsFalse(boleto2.EsTrasbordo, 
                "No debe ser trasbordo después de las 22:00");
            Assert.Less(tarjeta.Saldo, saldoDespuesPrimero);
        }

        [Test]
        public void Test_Trasbordo_LunesA21_59_Permite()
        {
            // Lunes 25 nov 2024, 21:59
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 21, 30, 0));
            
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            colectivo152.PagarCon(tarjeta);
            decimal saldoDespuesPrimero = tarjeta.Saldo;
            
            tiempo.AvanzarMinutos(10);
            
            Boleto boleto2 = colectivoK.PagarCon(tarjeta);

            Assert.IsTrue(boleto2.EsTrasbordo, 
                "Debe permitir trasbordo hasta las 22:00");
            Assert.AreEqual(saldoDespuesPrimero, tarjeta.Saldo);
        }

        #endregion

        #region Tests de Múltiples Trasbordos

        [Test]
        public void Test_TresViajes_DosTrasbordos_SinLimite()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            Boleto b1 = colectivo152.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(10);
            
            Boleto b2 = colectivoK.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(10);
            
            Boleto b3 = colectivo143.PagarCon(tarjeta);

            Assert.IsFalse(b1.EsTrasbordo, "Viaje 1: no es trasbordo");
            Assert.IsTrue(b2.EsTrasbordo, "Viaje 2: es trasbordo");
            Assert.IsTrue(b3.EsTrasbordo, "Viaje 3: es trasbordo");
            
            Assert.AreEqual(1580m, b1.MontoPagado);
            Assert.AreEqual(0m, b2.MontoPagado);
            Assert.AreEqual(0m, b3.MontoPagado);
            
            Assert.AreEqual(3420m, tarjeta.Saldo, 
                "Solo debe descontar el primer viaje");
        }

        [Test]
        public void Test_CincoTrasbordos_TodosGratis()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            // Primer viaje - paga
            Boleto b1 = colectivo152.PagarCon(tarjeta);
            Assert.AreEqual(1580m, b1.MontoPagado);
            
            // 5 trasbordos - todos gratis
            for (int i = 0; i < 5; i++)
            {
                tiempo.AvanzarMinutos(10);
                Colectivo colectivo = new Colectivo($"Línea{i}", "Empresa", tiempo);
                Boleto boleto = colectivo.PagarCon(tarjeta);
                
                Assert.IsTrue(boleto.EsTrasbordo, $"Trasbordo {i+1} debe ser gratis");
                Assert.AreEqual(0m, boleto.MontoPagado);
            }
            
            Assert.AreEqual(3420m, tarjeta.Saldo, 
                "Solo debe descontar el primer viaje");
        }

        [Test]
        public void Test_TrasbordoVolverAMismaLinea_CobraNormal()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            colectivo152.PagarCon(tarjeta); // Línea 152
            tiempo.AvanzarMinutos(10);
            
            colectivoK.PagarCon(tarjeta); // Línea K - trasbordo gratis
            tiempo.AvanzarMinutos(10);
            
            Boleto boleto3 = colectivo152.PagarCon(tarjeta); // Volver a 152

            Assert.IsFalse(boleto3.EsTrasbordo, 
                "Volver a la misma línea no es trasbordo");
            Assert.AreEqual(1580m, boleto3.MontoPagado);
        }

        #endregion

        #region Tests de Trasbordo con Franquicias

        [Test]
        public void Test_MedioBoleto_PuedeTrasbordar()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            Boleto b1 = colectivo152.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(10);
            
            Boleto b2 = colectivoK.PagarCon(tarjeta);

            Assert.IsNotNull(b1);
            Assert.IsNotNull(b2);
            Assert.IsFalse(b1.EsTrasbordo);
            Assert.IsTrue(b2.EsTrasbordo, "Medio boleto puede trasbordar");
            Assert.AreEqual(790m, b1.MontoPagado);
            Assert.AreEqual(0m, b2.MontoPagado);
        }

        [Test]
        public void Test_FranquiciaCompleta_PuedeTrasbordar()
        {
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta(tiempo);

            Boleto b1 = colectivo152.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(10);
            
            Boleto b2 = colectivoK.PagarCon(tarjeta);

            Assert.IsNotNull(b1);
            Assert.IsNotNull(b2);
            Assert.IsFalse(b1.EsTrasbordo);
            Assert.IsTrue(b2.EsTrasbordo, "Franquicia completa puede trasbordar");
            Assert.AreEqual(0m, b1.MontoPagado); // Primer viaje gratis
            Assert.AreEqual(0m, b2.MontoPagado); // Trasbordo gratis
        }

        #endregion

        #region Tests de Trasbordo Interurbano

        [Test]
        public void Test_DeUrbanoAInterurbano_PuedeTrasbordar()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            Boleto b1 = colectivo152.PagarCon(tarjeta); // Urbano
            decimal saldoDespues = tarjeta.Saldo;
            
            tiempo.AvanzarMinutos(10);
            
            Boleto b2 = colectivo500.PagarCon(tarjeta); // Interurbano

            Assert.IsNotNull(b1);
            Assert.IsNotNull(b2);
            Assert.IsFalse(b1.EsTrasbordo);
            Assert.IsTrue(b2.EsTrasbordo, 
                "Puede trasbordar de urbano a interurbano");
            Assert.AreEqual(0m, b2.MontoPagado);
            Assert.AreEqual(saldoDespues, tarjeta.Saldo);
        }

        [Test]
        public void Test_DeInterurbanoAUrbano_PuedeTrasbordar()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            Boleto b1 = colectivo500.PagarCon(tarjeta); // Interurbano
            decimal saldoDespues = tarjeta.Saldo;
            
            tiempo.AvanzarMinutos(10);
            
            Boleto b2 = colectivo152.PagarCon(tarjeta); // Urbano

            Assert.IsNotNull(b1);
            Assert.IsNotNull(b2);
            Assert.IsFalse(b1.EsTrasbordo);
            Assert.IsTrue(b2.EsTrasbordo, 
                "Puede trasbordar de interurbano a urbano");
            Assert.AreEqual(0m, b2.MontoPagado);
            Assert.AreEqual(saldoDespues, tarjeta.Saldo);
        }

        [Test]
        public void Test_DosInterurbanos_PuedeTrasbordar()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(10000m);

            Boleto b1 = colectivo500.PagarCon(tarjeta);
            decimal saldoDespues = tarjeta.Saldo;
            
            tiempo.AvanzarMinutos(10);
            
            ColectivoInterurbano colectivo501 = new ColectivoInterurbano("501", "Otra Empresa", tiempo);
            Boleto b2 = colectivo501.PagarCon(tarjeta);

            Assert.IsTrue(b2.EsTrasbordo, 
                "Puede trasbordar entre interurbanos");
            Assert.AreEqual(0m, b2.MontoPagado);
            Assert.AreEqual(saldoDespues, tarjeta.Saldo);
        }

        #endregion

        #region Tests del Boleto de Trasbordo

        [Test]
        public void Test_BoletoTrasbordo_MarcadoCorrectamente()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            Boleto b1 = colectivo152.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(10);
            Boleto b2 = colectivoK.PagarCon(tarjeta);

            Assert.IsFalse(b1.EsTrasbordo, "Primer boleto no es trasbordo");
            Assert.IsTrue(b2.EsTrasbordo, "Segundo boleto es trasbordo");
        }

        [Test]
        public void Test_BoletoTrasbordo_TieneFechaCorrecta()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            Boleto b1 = colectivo152.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(15);
            Boleto b2 = colectivoK.PagarCon(tarjeta);

            Assert.AreEqual(new DateTime(2024, 11, 25, 10, 0, 0), b1.FechaHora);
            Assert.AreEqual(new DateTime(2024, 11, 25, 10, 15, 0), b2.FechaHora);
        }

        #endregion
    }
}