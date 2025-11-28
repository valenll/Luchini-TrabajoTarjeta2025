/*using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    /// <summary>
    /// Tests para TiempoReal - verificar que devuelve tiempo del sistema
    /// </summary>
    [TestFixture]
    public class TiempoRealTest
    {
        [Test]
        public void Test_Ahora_DevuelveTiempoActual()
        {
            TiempoReal tiempo = new TiempoReal();
            
            DateTime antes = DateTime.Now;
            DateTime tiempoReal = tiempo.Ahora;
            DateTime despues = DateTime.Now;
            
            Assert.GreaterOrEqual(tiempoReal, antes);
            Assert.LessOrEqual(tiempoReal, despues);
        }

        [Test]
        public void Test_MultiplesSolicitudes_DevuelvenTiemposCercanos()
        {
            TiempoReal tiempo = new TiempoReal();
            
            DateTime tiempo1 = tiempo.Ahora;
            DateTime tiempo2 = tiempo.Ahora;
            
            TimeSpan diferencia = tiempo2 - tiempo1;
            
            // La diferencia debe ser mínima (menos de 1 segundo)
            Assert.Less(diferencia.TotalSeconds, 1);
        }

        [Test]
        public void Test_TiempoReal_NoEsConstante()
        {
            TiempoReal tiempo = new TiempoReal();
            
            DateTime tiempo1 = tiempo.Ahora;
            System.Threading.Thread.Sleep(10); // Esperar 10ms
            DateTime tiempo2 = tiempo.Ahora;
            
            // El tiempo debe avanzar
            Assert.Greater(tiempo2, tiempo1);
        }

        [Test]
        public void Test_TiempoReal_EsDelSistema()
        {
            TiempoReal tiempo = new TiempoReal();
            
            DateTime tiempoReal = tiempo.Ahora;
            DateTime tiempoSistema = DateTime.Now;
            
            // Deben ser muy cercanos (diferencia menor a 1 segundo)
            TimeSpan diferencia = (tiempoReal - tiempoSistema).Duration();
            Assert.Less(diferencia.TotalSeconds, 1);
        }

        [Test]
        public void Test_TiempoReal_TieneFechaActual()
        {
            TiempoReal tiempo = new TiempoReal();
            
            DateTime tiempoReal = tiempo.Ahora;
            DateTime hoy = DateTime.Now;
            
            Assert.AreEqual(hoy.Date, tiempoReal.Date);
        }

        [Test]
        public void Test_TiempoReal_ImplementaITiempoProvider()
        {
            ITiempoProvider tiempo = new TiempoReal();
            
            Assert.IsNotNull(tiempo.Ahora);
            Assert.IsInstanceOf<DateTime>(tiempo.Ahora);
        }

        [Test]
        public void Test_TiempoReal_UsadoEnBoleto()
        {
            TiempoReal tiempo = new TiempoReal();
            
            DateTime antes = DateTime.Now;
            Boleto boleto = new Boleto(1580m, "152", "Rosario Bus", 3420m, "Normal", 1, false, tiempo);
            DateTime despues = DateTime.Now;
            
            Assert.GreaterOrEqual(boleto.FechaHora, antes);
            Assert.LessOrEqual(boleto.FechaHora, despues);
        }

        [Test]
        public void Test_TiempoReal_UsadoEnTarjeta()
        {
            TiempoReal tiempo = new TiempoReal();
            Tarjeta tarjeta = new Tarjeta(tiempo);
            Colectivo colectivo = new Colectivo("152", "Rosario Bus", tiempo);
            
            tarjeta.Cargar(5000m);
            
            DateTime antes = DateTime.Now;
            Boleto boleto = colectivo.PagarCon(tarjeta);
            DateTime despues = DateTime.Now;
            
            Assert.IsNotNull(boleto);
            Assert.GreaterOrEqual(boleto.FechaHora, antes);
            Assert.LessOrEqual(boleto.FechaHora, despues);
        }

        [Test]
        public void Test_TiempoReal_MultiplesInstancias()
        {
            TiempoReal tiempo1 = new TiempoReal();
            TiempoReal tiempo2 = new TiempoReal();
            
            DateTime t1 = tiempo1.Ahora;
            DateTime t2 = tiempo2.Ahora;
            
            // Deben devolver tiempos muy cercanos
            TimeSpan diferencia = (t1 - t2).Duration();
            Assert.Less(diferencia.TotalSeconds, 1);
        }
    }
}*/
