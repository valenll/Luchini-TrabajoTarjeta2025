using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class PagarConMedioBoletoIntegracionTest
    {
        private TiempoSimulado tiempo;
        private Colectivo colectivo152;
        private Colectivo colectivoK;
        private Colectivo colectivo143;

        [SetUp]
        public void Setup()
        {
            // Lunes 25 nov 2024, 10:00 AM (horario válido para medio boleto: L-V 6-22)
            tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            colectivo152 = new Colectivo("152", "Rosario Bus", tiempo);
            colectivoK = new Colectivo("K", "Las Delicias", tiempo);
            colectivo143 = new Colectivo("143", "Semtur", tiempo);
        }

        #region Test 1: Dos boletos a mitad de precio

        [Test]
        public void Test_DosBoletos_MitadDePrecio_TiempoSuficiente_SaldoPositivo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);
            decimal saldoInicial = tarjeta.Saldo;

            // Primer viaje (misma línea para evitar trasbordo)
            Boleto boleto1 = colectivo152.PagarCon(tarjeta);
            decimal saldoDespuesPrimero = tarjeta.Saldo;
            
            // Esperar 5 minutos (tiempo mínimo requerido)
            tiempo.AvanzarMinutos(5);
            
            // Segundo viaje (MISMA línea para evitar trasbordo)
            Boleto boleto2 = colectivo152.PagarCon(tarjeta);
            decimal saldoDespuesSegundo = tarjeta.Saldo;

            Assert.IsNotNull(boleto1, "Primer boleto debe emitirse");
            Assert.IsNotNull(boleto2, "Segundo boleto debe emitirse");
            
            Assert.AreEqual(790m, boleto1.MontoPagado, 
                "Primer viaje debe costar la mitad (790)");
            Assert.AreEqual(790m, boleto2.MontoPagado, 
                "Segundo viaje debe costar la mitad (790)");
            
            Assert.AreEqual(saldoInicial - 790m, saldoDespuesPrimero, 
                "Saldo después del primer viaje");
            Assert.AreEqual(saldoInicial - 1580m, saldoDespuesSegundo, 
                "Saldo después del segundo viaje");
            
            Assert.AreEqual("Medio Boleto", boleto1.TipoTarjeta);
            Assert.AreEqual("Medio Boleto", boleto2.TipoTarjeta);
        }

        [Test]
        public void Test_DosBoletos_NoPermiteSiNoHanPasado5Minutos()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto1 = colectivo152.PagarCon(tarjeta);
            decimal saldoDespuesPrimero = tarjeta.Saldo;
            
            // Esperar solo 4 minutos (menos del mínimo)
            tiempo.AvanzarMinutos(4);
            
            Boleto boleto2 = colectivo152.PagarCon(tarjeta);

            Assert.IsNotNull(boleto1, "Primer boleto debe emitirse");
            Assert.IsNull(boleto2, 
                "Segundo boleto NO debe emitirse antes de 5 minutos");
            Assert.AreEqual(saldoDespuesPrimero, tarjeta.Saldo, 
                "El saldo no debe cambiar si falla el pago");
        }

        [Test]
        public void Test_DosBoletos_ConSaldoNegativo_Permite()
        {
            // Tarjeta con saldo que quedará negativo
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1500m); // Dejar 500 (menos que 2 viajes)

            Boleto boleto1 = colectivo152.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1, "Primer viaje debe poder pagarse");
            Assert.Less(tarjeta.Saldo, 0, "Saldo debe quedar negativo");
            
            tiempo.AvanzarMinutos(5);
            
            // Usar MISMA línea para evitar trasbordo
            Boleto boleto2 = colectivo152.PagarCon(tarjeta);

            Assert.IsNotNull(boleto2, 
                "Segundo viaje debe poder pagarse con saldo negativo");
            Assert.AreEqual(790m, boleto1.MontoPagado);
            Assert.AreEqual(790m, boleto2.MontoPagado);
            Assert.Less(tarjeta.Saldo, 0, "Saldo final debe ser negativo");
            Assert.GreaterOrEqual(tarjeta.Saldo, -1200m, 
                "Saldo no debe exceder límite negativo");
        }
        
        [Test]
        public void Test_DosBoletos_ExcedeLimiteNegativo_NoPermite()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1989m); // Dejar 11
            
            // Primer viaje (790) deja saldo en -779
            Boleto boleto1 = colectivo152.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1);
            Assert.AreEqual(-779m, tarjeta.Saldo);
            
            // Esperar más de 1 hora para que NO sea trasbordo
            tiempo.AvanzarMinutos(61);
            
            // Segundo viaje (790) dejaría saldo en -1569 (excede -1200)
            Boleto boleto2 = colectivo152.PagarCon(tarjeta);

            Assert.IsNull(boleto2, 
                "No debe permitir si excede el límite de saldo negativo");
            Assert.AreEqual(-779m, tarjeta.Saldo, 
                "El saldo no debe cambiar");
        }

        #endregion

        #region Test 2: Dos medio boleto y tercero completo

        [Test]
        public void Test_TresViajes_DosMediadPrecioYTerceroCompleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);
            decimal saldoInicial = tarjeta.Saldo;

            // Primer viaje (mitad de precio) - MISMA línea siempre
            Boleto boleto1 = colectivo152.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(5);
            
            // Segundo viaje (mitad de precio)
            Boleto boleto2 = colectivo152.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(5);
            
            // Tercer viaje (precio completo)
            Boleto boleto3 = colectivo152.PagarCon(tarjeta);

            Assert.IsNotNull(boleto1);
            Assert.IsNotNull(boleto2);
            Assert.IsNotNull(boleto3);
            
            Assert.AreEqual(790m, boleto1.MontoPagado, 
                "Primer viaje: mitad de precio");
            Assert.AreEqual(790m, boleto2.MontoPagado, 
                "Segundo viaje: mitad de precio");
            Assert.AreEqual(1580m, boleto3.MontoPagado, 
                "Tercer viaje: precio completo");
            
            decimal totalGastado = 790m + 790m + 1580m;
            Assert.AreEqual(saldoInicial - totalGastado, tarjeta.Saldo, 
                "Saldo final debe reflejar los 3 viajes");
        }

        [Test]
        public void Test_TresViajes_ConSaldoNegativo_TerceroCompleto()
        {
            // Saldo justo para 3 viajes pero quedando negativo
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(3000m);

            // Todos en la MISMA línea
            colectivo152.PagarCon(tarjeta); // -790
            tiempo.AvanzarMinutos(5);
            
            colectivo152.PagarCon(tarjeta); // -790
            tiempo.AvanzarMinutos(5);
            
            Boleto boleto3 = colectivo152.PagarCon(tarjeta); // -1580

            Assert.IsNotNull(boleto3, 
                "Tercer viaje debe poder pagarse aunque quede negativo");
            Assert.AreEqual(1580m, boleto3.MontoPagado, 
                "Tercer viaje debe ser precio completo");
            Assert.Less(tarjeta.Saldo, 0, "Saldo debe quedar negativo");
            Assert.AreEqual(-160m, tarjeta.Saldo, 
                "3000 - 790 - 790 - 1580 = -160");
        }

        [Test]
        public void Test_NuevoDia_ReiniciaContadorMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(10000m);

            // Día 1: 3 viajes en MISMA línea
            Boleto boleto1 = colectivo152.PagarCon(tarjeta); // Medio boleto
            tiempo.AvanzarMinutos(5);
            Boleto boleto2 = colectivo152.PagarCon(tarjeta); // Medio boleto
            tiempo.AvanzarMinutos(5);
            Boleto boleto3 = colectivo152.PagarCon(tarjeta); // Completo

            Assert.AreEqual(790m, boleto1.MontoPagado, 
                "Nuevo día: primer viaje medio boleto");
            Assert.AreEqual(790m, boleto2.MontoPagado, 
                "Nuevo día: segundo viaje medio boleto");
            Assert.AreEqual(1580m, boleto3.MontoPagado, 
                "Nuevo día: tercer viaje completo");
            
            // Cambiar al día siguiente
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 26, 10, 0, 0));
            
            // Día 2: Debería reiniciar
            Boleto boleto4 = colectivo152.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(5);
            Boleto boleto5 = colectivo152.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(5);
            Boleto boleto6 = colectivo152.PagarCon(tarjeta);

            Assert.AreEqual(790m, boleto4.MontoPagado, 
                "Nuevo día: primer viaje medio boleto");
            Assert.AreEqual(790m, boleto5.MontoPagado, 
                "Nuevo día: segundo viaje medio boleto");
            Assert.AreEqual(1580m, boleto6.MontoPagado, 
                "Nuevo día: tercer viaje completo");
        }

        #endregion

        #region Test 3: Trasbordo con Medio Boleto

        [Test]
        public void Test_Trasbordo_TienePrioridad_SobreMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            // Primer viaje paga medio boleto
            Boleto boleto1 = colectivo152.PagarCon(tarjeta);
            Assert.AreEqual(790m, boleto1.MontoPagado, 
                "Primer viaje paga medio boleto");
            Assert.IsFalse(boleto1.EsTrasbordo);
            
            decimal saldoDespuesPrimero = tarjeta.Saldo;
            
            // Esperar 10 minutos (más de 5 para medio boleto)
            tiempo.AvanzarMinutos(10);
            
            // Segundo viaje: cumple requisitos de trasbordo (línea diferente)
            Boleto boleto2 = colectivoK.PagarCon(tarjeta);

            // Trasbordo tiene prioridad
            Assert.IsNotNull(boleto2);
            Assert.IsTrue(boleto2.EsTrasbordo, 
                "Debe ser trasbordo (tiene prioridad)");
            Assert.AreEqual(0m, boleto2.MontoPagado, 
                "Trasbordo es gratis, no paga medio boleto");
            Assert.AreEqual(saldoDespuesPrimero, tarjeta.Saldo, 
                "Saldo no cambia en trasbordo");
        }

        [Test]
        public void Test_Trasbordo_DespuesDe1Hora_PagaMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            colectivo152.PagarCon(tarjeta); // Primer viaje
            decimal saldoDespuesPrimero = tarjeta.Saldo;
            
            // Esperar más de 1 hora (ya no es trasbordo)
            tiempo.AvanzarMinutos(61);
            
            Boleto boleto2 = colectivoK.PagarCon(tarjeta);

            // No es trasbordo, paga medio boleto
            Assert.IsNotNull(boleto2);
            Assert.IsFalse(boleto2.EsTrasbordo, 
                "Después de 1 hora no es trasbordo");
            Assert.AreEqual(790m, boleto2.MontoPagado, 
                "Debe pagar medio boleto (segundo del día)");
            Assert.AreEqual(saldoDespuesPrimero - 790m, tarjeta.Saldo);
        }

        [Test]
        public void Test_Trasbordo_MismaLinea_PagaMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            colectivo152.PagarCon(tarjeta);
            decimal saldoDespuesPrimero = tarjeta.Saldo;
            
            tiempo.AvanzarMinutos(10);
            
            // Intentar trasbordo en la misma línea
            Boleto boleto2 = colectivo152.PagarCon(tarjeta);

            // No es trasbordo (misma línea), paga medio boleto
            Assert.IsNotNull(boleto2);
            Assert.IsFalse(boleto2.EsTrasbordo, 
                "Misma línea no es trasbordo");
            Assert.AreEqual(790m, boleto2.MontoPagado, 
                "Debe pagar medio boleto");
            Assert.AreEqual(saldoDespuesPrimero - 790m, tarjeta.Saldo);
        }

        [Test]
        public void Test_Trasbordo_FueradeHorario_NoPermiteTrasbordo_PagaMedioBoleto()
        {
            // Horario donde medio boleto funciona pero trasbordo no
            // Trasbordo: L-S 7:00-22:00
            // Medio boleto: L-V 6:00-22:00
            // Entonces a las 6:30 AM lunes: medio boleto SÍ, trasbordo NO
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 6, 30, 0));
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto1 = colectivo152.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1, "Medio boleto funciona a las 6:30");
            decimal saldoDespues = tarjeta.Saldo;
            
            tiempo.AvanzarMinutos(10); // 6:40 AM
            
            Boleto boleto2 = colectivoK.PagarCon(tarjeta);

            Assert.IsNotNull(boleto2);
            Assert.IsFalse(boleto2.EsTrasbordo, 
                "Fuera de horario de trasbordo (antes de las 7)");
            Assert.AreEqual(790m, boleto2.MontoPagado, 
                "Paga medio boleto");
            Assert.AreEqual(saldoDespues - 790m, tarjeta.Saldo);
        }

        [Test]
        public void Test_VariosTrasbordos_DespuesPagaMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(10000m);

            // Primer viaje paga medio boleto
            Boleto b1 = colectivo152.PagarCon(tarjeta);
            Assert.AreEqual(790m, b1.MontoPagado);
            Assert.IsFalse(b1.EsTrasbordo);
            
            // Dos trasbordos gratis
            tiempo.AvanzarMinutos(10);
            Boleto b2 = colectivoK.PagarCon(tarjeta);
            Assert.IsTrue(b2.EsTrasbordo);
            Assert.AreEqual(0m, b2.MontoPagado);
            
            tiempo.AvanzarMinutos(10);
            Boleto b3 = colectivo143.PagarCon(tarjeta);
            Assert.IsTrue(b3.EsTrasbordo);
            Assert.AreEqual(0m, b3.MontoPagado);
            
            // Esperar más de 1 hora para que expire trasbordo
            tiempo.AvanzarMinutos(50); // Total: 70 minutos desde b1
            
            // Siguiente viaje debe pagar medio boleto (es el segundo del día)
            // Usar línea diferente a las anteriores para evitar conflictos
            Colectivo colectivo133 = new Colectivo("133", "Empresa", tiempo);
            Boleto b4 = colectivo133.PagarCon(tarjeta);

            Assert.IsNotNull(b4);
            Assert.IsFalse(b4.EsTrasbordo);
            Assert.AreEqual(790m, b4.MontoPagado, 
                "Segundo medio boleto del día (los trasbordos no cuentan)");
        }

        #endregion

    }
}

/*
        [Test]
        public void Test_Domingo_NoPermiteViaje()
        {
            // Arrange: Configurar domingo
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 24, 10, 0, 0));
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);
            decimal saldoInicial = tarjeta.Saldo;

            // Act
            Boleto boleto = colectivo152.PagarCon(tarjeta);

            // Assert
            Assert.IsNull(boleto, 
                "No debe permitir viaje en domingo con medio boleto");
            Assert.AreEqual(saldoInicial, tarjeta.Saldo, 
                "Saldo no debe cambiar");
        }
        
        [Test]
        public void Test_FueraDeHorario_NoPermiteViaje()
        {
            // Arrange: Lunes pero a las 5:00 AM (antes de las 6)
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 5, 0, 0));
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            // Act
            Boleto boleto = colectivo152.PagarCon(tarjeta);

            // Assert
            Assert.IsNull(boleto, 
                "No debe permitir viaje antes de las 6:00");
        }

        #endregion

        #region Tests Adicionales de Casos Complejos

        [Test]
        public void Test_EscenarioCompleto_ConTodosMedioBoletoYTrasbordoYCompleto()
        {
            // Arrange
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(10000m);
            decimal saldoInicial = tarjeta.Saldo;

            // Escenario:
            // 1. Viaje normal - medio boleto (790) - línea 152
            // 2. Trasbordo gratis (0) - línea K
            // 3. Trasbordo gratis (0) - línea 143
            // 4. Esperar >1h, viaje normal - medio boleto (790) - línea nueva
            // 5. Viaje normal sin esperar 5 min - precio completo (1580) - misma línea
            // 6. Esperar para que expire trasbordo, viaje normal - completo (1580)
            
            // Act & Assert
            
            // Viaje 1: medio boleto (primero del día)
            Boleto b1 = colectivo152.PagarCon(tarjeta);
            Assert.AreEqual(790m, b1.MontoPagado);
            Assert.IsFalse(b1.EsTrasbordo);
            
            // Viaje 2: trasbordo
            tiempo.AvanzarMinutos(10);
            Boleto b2 = colectivoK.PagarCon(tarjeta);
            Assert.AreEqual(0m, b2.MontoPagado);
            Assert.IsTrue(b2.EsTrasbordo);
            
            // Viaje 3: trasbordo
            tiempo.AvanzarMinutos(10);
            Boleto b3 = colectivo143.PagarCon(tarjeta);
            Assert.AreEqual(0m, b3.MontoPagado);
            Assert.IsTrue(b3.EsTrasbordo);
            
            // Esperar >1h desde b1 (total 70 min)
            tiempo.AvanzarMinutos(50);
            
            // Viaje 4: segundo medio boleto (nueva línea)
            Colectivo colectivo133 = new Colectivo("133", "Empresa", tiempo);
            Boleto b4 = colectivo133.PagarCon(tarjeta);
            Assert.AreEqual(790m, b4.MontoPagado, "Segundo medio boleto del día");
            Assert.IsFalse(b4.EsTrasbordo);
            
            // Viaje 5: sin esperar 5 min - PERO como ya usó los 2 medio boletos,
            // este viaje es a tarifa completa y NO tiene restricción de 5 minutos
            tiempo.AvanzarMinutos(2);
            Boleto b5 = colectivo133.PagarCon(tarjeta);
            Assert.IsNotNull(b5, "Los viajes a tarifa completa no tienen restricción de 5 minutos");
            Assert.AreEqual(1580m, b5.MontoPagado, "Tercer viaje del día es a tarifa completa");
            Assert.IsFalse(b5.EsTrasbordo);
            
            // Viaje 6: después de 5 min desde b5, otro viaje completo
            tiempo.AvanzarMinutos(70);
            
            // Usar línea diferente
            Colectivo colectivo120 = new Colectivo("120", "Nueva", tiempo);
            Boleto b6 = colectivo120.PagarCon(tarjeta);
            Assert.AreEqual(1580m, b6.MontoPagado, "Cuarto viaje es completo");
            Assert.IsFalse(b6.EsTrasbordo);
            
            // Verificar saldo final: 2 medio boletos + 2 completos
            decimal totalGastado = 790m + 790m + 1580m + 1580m; // Los trasbordos no cuestan
            Assert.AreEqual(saldoInicial - totalGastado, tarjeta.Saldo);
        }

        #endregion
    }
}
*/