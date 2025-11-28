/*using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    /// <summary>
    /// Tests para aumentar cobertura de TiempoSimulado
    /// </summary>
    [TestFixture]
    public class TiempoSimuladoTest
    {
        [Test]
        public void Test_ConstructorSinParametros_UsaTiempoActual()
        {
            DateTime antes = DateTime.Now;
            TiempoSimulado tiempo = new TiempoSimulado();
            DateTime despues = DateTime.Now;
            
            Assert.GreaterOrEqual(tiempo.Ahora, antes);
            Assert.LessOrEqual(tiempo.Ahora, despues);
        }

        [Test]
        public void Test_ConstructorConParametro_EstableceTiempoInicial()
        {
            DateTime fechaInicial = new DateTime(2024, 6, 15, 14, 30, 0);
            TiempoSimulado tiempo = new TiempoSimulado(fechaInicial);
            
            Assert.AreEqual(fechaInicial, tiempo.Ahora);
        }

        [Test]
        public void Test_EstablecerTiempo_CambiaTiempoActual()
        {
            TiempoSimulado tiempo = new TiempoSimulado(new DateTime(2024, 1, 1, 0, 0, 0));
            
            DateTime nuevoTiempo = new DateTime(2024, 12, 31, 23, 59, 59);
            tiempo.EstablecerTiempo(nuevoTiempo);
            
            Assert.AreEqual(nuevoTiempo, tiempo.Ahora);
        }

        [Test]
        public void Test_AvanzarTiempo_ConTimeSpan()
        {
            TiempoSimulado tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            
            tiempo.AvanzarTiempo(TimeSpan.FromMinutes(45));
            
            Assert.AreEqual(new DateTime(2024, 11, 25, 10, 45, 0), tiempo.Ahora);
        }

        [Test]
        public void Test_AvanzarTiempo_ConTimeSpanComplejo()
        {
            TiempoSimulado tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            
            tiempo.AvanzarTiempo(new TimeSpan(2, 30, 15)); // 2h 30m 15s
            
            Assert.AreEqual(new DateTime(2024, 11, 25, 12, 30, 15), tiempo.Ahora);
        }

        [Test]
        public void Test_AvanzarMinutos_FuncionaCorrectamente()
        {
            TiempoSimulado tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            
            tiempo.AvanzarMinutos(15.5); // 15 minutos y 30 segundos
            
            Assert.AreEqual(new DateTime(2024, 11, 25, 10, 15, 30), tiempo.Ahora);
        }

        [Test]
        public void Test_AvanzarHoras_FuncionaCorrectamente()
        {
            TiempoSimulado tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            
            tiempo.AvanzarHoras(3.5); // 3 horas y 30 minutos
            
            Assert.AreEqual(new DateTime(2024, 11, 25, 13, 30, 0), tiempo.Ahora);
        }

        [Test]
        public void Test_AvanzarDias_FuncionaCorrectamente()
        {
            TiempoSimulado tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            
            tiempo.AvanzarDias(5);
            
            Assert.AreEqual(new DateTime(2024, 11, 30, 10, 0, 0), tiempo.Ahora);
        }

        [Test]
        public void Test_AvanzarDias_CambiaDeMes()
        {
            TiempoSimulado tiempo = new TiempoSimulado(new DateTime(2024, 11, 28, 10, 0, 0));
            
            tiempo.AvanzarDias(5);
            
            Assert.AreEqual(new DateTime(2024, 12, 3, 10, 0, 0), tiempo.Ahora);
        }

        [Test]
        public void Test_AvanzarDias_CambiaDeAño()
        {
            TiempoSimulado tiempo = new TiempoSimulado(new DateTime(2024, 12, 30, 10, 0, 0));
            
            tiempo.AvanzarDias(3);
            
            Assert.AreEqual(new DateTime(2025, 1, 2, 10, 0, 0), tiempo.Ahora);
        }

        [Test]
        public void Test_MultiplesCambios_MantienenCoherencia()
        {
            TiempoSimulado tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            
            tiempo.AvanzarHoras(2);        // 12:00
            tiempo.AvanzarMinutos(30);     // 12:30
            tiempo.AvanzarDias(1);         // 26 nov, 12:30
            tiempo.AvanzarMinutos(15);     // 12:45
            
            Assert.AreEqual(new DateTime(2024, 11, 26, 12, 45, 0), tiempo.Ahora);
        }

        [Test]
        public void Test_EstablecerTiempo_SobrescribeAvances()
        {
            TiempoSimulado tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            
            tiempo.AvanzarHoras(5);
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 8, 0, 0));
            
            Assert.AreEqual(new DateTime(2024, 11, 25, 8, 0, 0), tiempo.Ahora);
        }

        [Test]
        public void Test_AvanzarTiempo_ConValoresNegativos()
        {
            TiempoSimulado tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            
            tiempo.AvanzarTiempo(TimeSpan.FromHours(-2));
            
            Assert.AreEqual(new DateTime(2024, 11, 25, 8, 0, 0), tiempo.Ahora);
        }

        [Test]
        public void Test_AvanzarMinutos_ConValorCero()
        {
            TiempoSimulado tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            DateTime tiempoOriginal = tiempo.Ahora;
            
            tiempo.AvanzarMinutos(0);
            
            Assert.AreEqual(tiempoOriginal, tiempo.Ahora);
        }

        [Test]
        public void Test_ImplementaITiempoProvider()
        {
            ITiempoProvider tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            
            Assert.IsNotNull(tiempo.Ahora);
            Assert.AreEqual(new DateTime(2024, 11, 25, 10, 0, 0), tiempo.Ahora);
        }

        [Test]
        public void Test_MultiplesInstancias_Independientes()
        {
            TiempoSimulado tiempo1 = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            TiempoSimulado tiempo2 = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            
            tiempo1.AvanzarHoras(2);
            
            Assert.AreEqual(new DateTime(2024, 11, 25, 12, 0, 0), tiempo1.Ahora);
            Assert.AreEqual(new DateTime(2024, 11, 25, 10, 0, 0), tiempo2.Ahora);
        }

        [Test]
        public void Test_UsadoEnBoleto_MantieneTiempoFijo()
        {
            TiempoSimulado tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            
            Boleto boleto1 = new Boleto(1580m, "152", "Rosario Bus", 3420m, "Normal", 1, false, tiempo);
            
            tiempo.AvanzarHoras(2);
            
            Boleto boleto2 = new Boleto(1580m, "K", "Las Delicias", 1840m, "Normal", 1, false, tiempo);
            
            Assert.AreEqual(new DateTime(2024, 11, 25, 10, 0, 0), boleto1.FechaHora);
            Assert.AreEqual(new DateTime(2024, 11, 25, 12, 0, 0), boleto2.FechaHora);
        }

        [Test]
        public void Test_CombinacionAvanzarYEstablecer()
        {
            TiempoSimulado tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            
            tiempo.AvanzarDias(1);
            Assert.AreEqual(new DateTime(2024, 11, 26, 10, 0, 0), tiempo.Ahora);
            
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 27, 14, 0, 0));
            Assert.AreEqual(new DateTime(2024, 11, 27, 14, 0, 0), tiempo.Ahora);
            
            tiempo.AvanzarMinutos(30);
            Assert.AreEqual(new DateTime(2024, 11, 27, 14, 30, 0), tiempo.Ahora);
        }
    }
}
*/