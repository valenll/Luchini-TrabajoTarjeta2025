using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class MiBiciTuBiciIntegracionTest
    {
        private TiempoSimulado tiempo;
        private EstacionBicicletas estacion;

        [SetUp]
        public void Setup()
        {
            // Lunes 25 nov 2024, 10:00 AM
            tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            estacion = new EstacionBicicletas(tiempo);
        }

        #region Test 1: Retiro exitoso

        [Test]
        public void Test_RetirarBici_Exitoso_TarifaCorrectaSinMultaSaldoSuficiente()
        {
            //Crear tarjeta con saldo suficiente
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);
            decimal saldoInicial = tarjeta.Saldo;

            //Retirar una bicicleta
            RetiroBici retiro = estacion.RetirarBici(tarjeta);

            //Verificar que el retiro fue exitoso
            Assert.IsNotNull(retiro, "El retiro debe ser exitoso");
            Assert.AreEqual(1777.50m, retiro.TarifaCobrada, 
                "La tarifa cobrada debe ser exactamente 1777.50");
            Assert.AreEqual(0m, retiro.MultasCobradas, 
                "No debe haber multas en el primer retiro");
            Assert.AreEqual(1777.50m, retiro.TotalCobrado, 
                "El total cobrado debe ser igual a la tarifa");
            Assert.AreEqual(saldoInicial - 1777.50m, tarjeta.Saldo, 
                "El saldo debe reducirse exactamente por la tarifa");
            Assert.IsFalse(retiro.FueDevuelta, 
                "La bici no debe estar marcada como devuelta");
        }

        /*
        [Test]
        public void Test_RetirarYDevolver_DentroDelTiempoLimite_SinMulta()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            // Act: Retirar bici
            RetiroBici retiro = estacion.RetirarBici(tarjeta);
            
            // Usar la bici por 45 minutos (dentro del límite de 60)
            tiempo.AvanzarMinutos(45);
            estacion.DevolverBici(tarjeta.Id);

            // Assert
            Assert.IsTrue(retiro.FueDevuelta, "La bici debe estar devuelta");
            Assert.AreEqual(45, retiro.TiempoDeUso.TotalMinutes, 
                "El tiempo de uso debe ser 45 minutos");
            
            // Verificar que no hay multas pendientes
            decimal multas = estacion.CalcularMultasPendientes(tarjeta.Id);
            Assert.AreEqual(0m, multas, 
                "No debe haber multas por uso dentro del tiempo límite");
        }
        */

        /*
        [Test]
        public void Test_RetirarBici_ConSaldoExacto()
        {
            // Arrange: Cargar exactamente la tarifa
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(2000m); // Carga mínima que cubre la tarifa
            
            // Act
            RetiroBici retiro = estacion.RetirarBici(tarjeta);

            // Assert
            Assert.IsNotNull(retiro, "Debe poder retirar con saldo exacto");
            Assert.AreEqual(222.50m, tarjeta.Saldo, "El saldo restante debe ser correcto");
        }
        */

        #endregion

        #region Test 2: No se puede retirar

        [Test]
        public void Test_NoRetirar_SaldoInsuficiente_SinMultas()
        {
            // Tarjeta con saldo que excede el límite negativo
            // Saldo actual: 600, Costo: 1777.50, Saldo final: -1177.50
            // Pero límite es -1200, entonces debería poder
            // Para que falle: Saldo: 500, Costo: 1777.50, Saldo final: -1277.50 (excede -1200)
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1550m); // Dejar saldo en 450
            decimal saldoAntes = tarjeta.Saldo;
            
            // 450 - 1777.50 = -1327.50 (excede el límite de -1200)

            RetiroBici retiro = estacion.RetirarBici(tarjeta);

            Assert.IsNull(retiro, "No debe poder retirar sin saldo suficiente");
            Assert.AreEqual(saldoAntes, tarjeta.Saldo, 
                "El saldo no debe cambiar si falla el retiro");
        }

        [Test]
        public void Test_NoRetirar_SaldoInsuficiente_ConMultaPendiente()
        {
            // Usar bici con exceso de tiempo primero
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);
            
            // Primer retiro: usar por 90 minutos (30 min de exceso)
            RetiroBici retiro1 = estacion.RetirarBici(tarjeta);
            tiempo.AvanzarMinutos(90);
            estacion.DevolverBici(tarjeta.Id);
            
            // Verificar que hay multa pendiente
            decimal multaPendiente = estacion.CalcularMultasPendientes(tarjeta.Id);
            Assert.AreEqual(1000m, multaPendiente, "Debe haber 1 multa de 1000");
            
            // Gastar el saldo para dejarlo en un valor que exceda el límite negativo
            // Saldo actual: ~3222.50 (5000 - 1777.50)
            // Necesita: 1777.50 + 1000 = 2777.50
            // Dejar saldo en: 2777.50 - 1200 - 1 = 1576.50
            tarjeta.Descontar(1646m); // Dejar ~1576.50
            decimal saldoAntes = tarjeta.Saldo;
            
            // Intentar retirar otra bici (necesita tarifa + multa = 2777.50)
            // Saldo final sería: ~1576.50 - 2777.50 = -1201 (excede -1200)
            RetiroBici retiro2 = estacion.RetirarBici(tarjeta);
            
            Assert.IsNull(retiro2, 
                "No debe poder retirar si no alcanza para tarifa + multa");
            Assert.AreEqual(saldoAntes, tarjeta.Saldo, 
                "El saldo no debe cambiar");
        }

        /*
        [Test]
        public void Test_NoRetirar_SaldoNegativoInsuficiente()
        {
            // Arrange: Tarjeta con saldo negativo que excede límite
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(2700m); // Saldo negativo -700
            decimal saldoAntes = tarjeta.Saldo;
            
            // -700 - 1777.50 = -2477.50 (excede límite de -1200)

            // Act
            RetiroBici retiro = estacion.RetirarBici(tarjeta);

            // Assert
            Assert.IsNull(retiro, 
                "No debe poder retirar con saldo negativo insuficiente");
            Assert.AreEqual(saldoAntes, tarjeta.Saldo);
        }
        */

        /*
        [Test]
        public void Test_NoRetirar_ConVariasMultasAcumuladas()
        {
            // Arrange: Acumular varias multas en el día
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(10000m);
            
            // Primera bici: 2 horas de exceso (2 multas)
            estacion.RetirarBici(tarjeta);
            tiempo.AvanzarMinutos(180); // 3 horas
            estacion.DevolverBici(tarjeta.Id);
            
            // Verificar multas pendientes después de la primera bici
            decimal multasDespuesPrimera = estacion.CalcularMultasPendientes(tarjeta.Id);
            Assert.AreEqual(2000m, multasDespuesPrimera, "Debe haber 2 multas de la primera bici");
            
            // Segunda bici: se cobran las 2 multas anteriores + tarifa
            tiempo.AvanzarMinutos(10);
            estacion.RetirarBici(tarjeta); // Aquí se cobran las 2 multas anteriores
            tiempo.AvanzarMinutos(120); // 2 horas (1 hora de exceso = 1 multa nueva)
            estacion.DevolverBici(tarjeta.Id);
            
            // Ahora solo debe haber 1 multa pendiente (la de la segunda bici)
            // porque las 2 anteriores ya fueron cobradas en el segundo retiro
            decimal multas = estacion.CalcularMultasPendientes(tarjeta.Id);
            Assert.AreEqual(1000m, multas, "Debe haber 1 multa acumulada (la de la segunda bici)");
            
            // Gastar saldo para que no alcance
            // Saldo actual: 10000 - 1777.50 (retiro 1) - 2000 (multas cobradas en retiro 2) - 1777.50 (retiro 2) = 4445
            // Necesita: 1777.50 + 1000 = 2777.50
            // Dejar en: 2777.50 - 1200 - 1 = 1576.50
            decimal saldoActual = tarjeta.Saldo;
            tarjeta.Descontar(saldoActual - 1576m);
            
            // Act: Intentar retirar (necesita 2777.50, tiene 1576.50)
            // Saldo final: 1576.50 - 2777.50 = -1201 (excede -1200)
            RetiroBici retiro3 = estacion.RetirarBici(tarjeta);
            
            // Assert
            Assert.IsNull(retiro3, 
                "No debe poder retirar con saldo insuficiente para tarifa + multas");
        }
        */

        #endregion

        #region Test 3: Retiro con multas acumuladas

        [Test]
        public void Test_RetirarBici_ConUnaMultaAcumulada()
        {
            // Usar bici con exceso de tiempo
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(10000m);
            
            // Primer retiro: usar por 90 minutos (30 min de exceso = 1 multa)
            RetiroBici retiro1 = estacion.RetirarBici(tarjeta);
            tiempo.AvanzarMinutos(90);
            estacion.DevolverBici(tarjeta.Id);
            
            decimal saldoDespuesPrimero = tarjeta.Saldo;
            decimal multaPendiente = estacion.CalcularMultasPendientes(tarjeta.Id);
            
            // Segundo retiro debe cobrar tarifa + multa
            tiempo.AvanzarMinutos(10);
            RetiroBici retiro2 = estacion.RetirarBici(tarjeta);
            
            Assert.IsNotNull(retiro2, "Debe poder retirar con saldo suficiente");
            Assert.AreEqual(1777.50m, retiro2.TarifaCobrada, 
                "La tarifa debe ser la estándar");
            Assert.AreEqual(1000m, retiro2.MultasCobradas, 
                "Debe cobrar 1 multa acumulada");
            Assert.AreEqual(2777.50m, retiro2.TotalCobrado, 
                "Total = tarifa + multa");
            Assert.AreEqual(saldoDespuesPrimero - 2777.50m, tarjeta.Saldo,
                "Debe descontar tarifa + multa");
            
            // Verificar que ya no hay multas pendientes
            decimal multasDespues = estacion.CalcularMultasPendientes(tarjeta.Id);
            Assert.AreEqual(0m, multasDespues, 
                "Las multas deben haberse cobrado");
        }

        [Test]
        public void Test_RetirarBici_ConVariasMultasAcumuladas()
        {
            // Acumular varias multas
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(15000m);
            
            // Primera bici: 2 horas de exceso (2 multas)
            estacion.RetirarBici(tarjeta);
            tiempo.AvanzarMinutos(180); // 3 horas
            estacion.DevolverBici(tarjeta.Id);
            
            // Verificar que hay 2 multas pendientes
            decimal multasDespuesPrimera = estacion.CalcularMultasPendientes(tarjeta.Id);
            Assert.AreEqual(2000m, multasDespuesPrimera, "Debe haber 2 multas después de la primera bici");
            
            // Segunda bici: se cobran las 2 multas anteriores
            tiempo.AvanzarMinutos(10);
            RetiroBici retiro2 = estacion.RetirarBici(tarjeta);
            Assert.AreEqual(2000m, retiro2.MultasCobradas, "Debe cobrar las 2 multas en el segundo retiro");
            
            tiempo.AvanzarMinutos(120); // 2 horas (1 hora de exceso = 1 multa)
            estacion.DevolverBici(tarjeta.Id);
            
            decimal saldoAntes = tarjeta.Saldo;
            decimal multasAcumuladas = estacion.CalcularMultasPendientes(tarjeta.Id);
            Assert.AreEqual(1000m, multasAcumuladas, "Debe haber 1 multa (la de la segunda bici)");
            
            // Tercer retiro
            tiempo.AvanzarMinutos(10);
            RetiroBici retiro3 = estacion.RetirarBici(tarjeta);
            
            Assert.IsNotNull(retiro3);
            Assert.AreEqual(1777.50m, retiro3.TarifaCobrada);
            Assert.AreEqual(1000m, retiro3.MultasCobradas, 
                "Debe cobrar 1 multa acumulada (la de la segunda bici)");
            Assert.AreEqual(2777.50m, retiro3.TotalCobrado);
            Assert.AreEqual(saldoAntes - 2777.50m, tarjeta.Saldo);
        }

        /*
        [Test]
        public void Test_MultasDelDiaAnterior_NoSeCobran()
        {
            // Arrange: Generar multa en día anterior
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(10000m);
            
            // Retiro con exceso de tiempo
            estacion.RetirarBici(tarjeta);
            tiempo.AvanzarMinutos(120); // 2 horas (1 hora de exceso)
            estacion.DevolverBici(tarjeta.Id);
            
            // Cambiar al día siguiente
            tiempo.AvanzarDias(1);
            
            decimal saldoAntes = tarjeta.Saldo;
            
            // Act: Retirar en el nuevo día
            RetiroBici retiro = estacion.RetirarBici(tarjeta);
            
            // Assert: No debe cobrar multas del día anterior
            Assert.IsNotNull(retiro);
            Assert.AreEqual(0m, retiro.MultasCobradas, 
                "No debe cobrar multas de días anteriores");
            Assert.AreEqual(1777.50m, retiro.TotalCobrado);
            Assert.AreEqual(saldoAntes - 1777.50m, tarjeta.Saldo);
        }
        */

        /*
        [Test]
        public void Test_CalculoMulta_ExactamenteUnaHoraExtra()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(10000m);
            
            // Act: Usar exactamente 120 minutos (60 extra)
            estacion.RetirarBici(tarjeta);
            tiempo.AvanzarMinutos(120);
            estacion.DevolverBici(tarjeta.Id);
            
            // Assert
            decimal multa = estacion.CalcularMultasPendientes(tarjeta.Id);
            Assert.AreEqual(1000m, multa, 
                "1 hora extra = 1 multa de 1000");
        }
        */

        /*
        [Test]
        public void Test_CalculoMulta_UnaHoraYMediaExtra()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(10000m);
            
            // Act: Usar 150 minutos (90 extra = 1.5 horas)
            estacion.RetirarBici(tarjeta);
            tiempo.AvanzarMinutos(150);
            estacion.DevolverBici(tarjeta.Id);
            
            // Assert: Debe redondear hacia arriba
            decimal multa = estacion.CalcularMultasPendientes(tarjeta.Id);
            Assert.AreEqual(2000m, multa, 
                "1.5 horas extra = 2 multas (redondeo hacia arriba)");
        }
        */

        #endregion

        /*
        [Test]
        public void Test_TarjetaNormal_PuedeUsarBici()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);
            
            RetiroBici retiro = estacion.RetirarBici(tarjeta);
            
            Assert.IsNotNull(retiro);
            Assert.AreEqual(1777.50m, retiro.TarifaCobrada);
        }
        */

        /*
        [Test]
        public void Test_MedioBoleto_PuedeUsarBici_SinDescuento()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);
            
            RetiroBici retiro = estacion.RetirarBici(tarjeta);
            
            Assert.IsNotNull(retiro, "Medio boleto puede usar bicis");
            Assert.AreEqual(1777.50m, retiro.TarifaCobrada, 
                "NO hay descuento para bicis");
        }
        */

        /*
        [Test]
        public void Test_FranquiciaCompleta_PuedeUsarBici_SinDescuento()
        {
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta(tiempo);
            tarjeta.Cargar(5000m);
            
            RetiroBici retiro = estacion.RetirarBici(tarjeta);
            
            Assert.IsNotNull(retiro, "Franquicia completa puede usar bicis");
            Assert.AreEqual(1777.50m, retiro.TarifaCobrada, 
                "NO hay descuento para bicis");
        }
        */

        /*
        [Test]
        public void Test_BoletoGratuito_PuedeUsarBici_SinDescuento()
        {
            TarjetaBoletoGratuitoEstudiantil tarjeta = 
                new TarjetaBoletoGratuitoEstudiantil(tiempo);
            tarjeta.Cargar(5000m);
            
            RetiroBici retiro = estacion.RetirarBici(tarjeta);
            
            Assert.IsNotNull(retiro, "Boleto gratuito puede usar bicis");
            Assert.AreEqual(1777.50m, retiro.TarifaCobrada, 
                "NO hay descuento para bicis");
        }
        */

        /*
        [Test]
        public void Test_VariosRetirosEnElMismoDia_TodosConTarifaNormal()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(15000m);
            
            // Primer retiro
            RetiroBici r1 = estacion.RetirarBici(tarjeta);
            tiempo.AvanzarMinutos(30);
            estacion.DevolverBici(tarjeta.Id);
            
            // Segundo retiro
            tiempo.AvanzarMinutos(10);
            RetiroBici r2 = estacion.RetirarBici(tarjeta);
            tiempo.AvanzarMinutos(30);
            estacion.DevolverBici(tarjeta.Id);
            
            // Tercer retiro
            tiempo.AvanzarMinutos(10);
            RetiroBici r3 = estacion.RetirarBici(tarjeta);
            
            Assert.AreEqual(1777.50m, r1.TarifaCobrada);
            Assert.AreEqual(1777.50m, r2.TarifaCobrada);
            Assert.AreEqual(1777.50m, r3.TarifaCobrada);
        }
        */

        /*
        [Test]
        public void Test_SaldoNegativo_PuedeUsarBici()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1500m); // Dejar 500
            
            // 500 - 1777.50 = -1277.50
            // Pero el límite es -1200, entonces NO debería poder
            
            RetiroBici retiro = estacion.RetirarBici(tarjeta);
            
            Assert.IsNull(retiro, 
                "No debe poder usar si excede el límite negativo");
        }
        */

        /*
        [Test]
        public void Test_SaldoNegativo_DentroDelLimite_PuedeUsarBici()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1400m); // Dejar 600
            
            // 600 - 1777.50 = -1177.50 (dentro del límite de -1200)
            
            RetiroBici retiro = estacion.RetirarBici(tarjeta);
            
            Assert.IsNotNull(retiro, 
                "Debe poder usar con saldo que queda negativo dentro del límite");
            Assert.Less(tarjeta.Saldo, 0, "El saldo debe quedar negativo");
            Assert.GreaterOrEqual(tarjeta.Saldo, -1200m, 
                "No debe exceder el límite de saldo negativo");
        }
        */

    }
}
