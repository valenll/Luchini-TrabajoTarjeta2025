/*using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class ColectivoInterurbanoTest
    {
        private TiempoSimulado tiempo;
        private ColectivoInterurbano colectivoInterurbano;
        private Colectivo colectivoUrbano;
        private const decimal TARIFA_INTERURBANA = 3000m;
        private const decimal TARIFA_URBANA = 1580m;

        [SetUp]
        public void Setup()
        {
            // Lunes 25 nov 2024, 10:00 AM (horario permitido)
            tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            colectivoInterurbano = new ColectivoInterurbano("500", "Galvez Express", tiempo);
            colectivoUrbano = new Colectivo("152", "Rosario Bus", tiempo);
        }

        #region Tests de Tarifa Interurbana

        [Test]
        public void Test_TarjetaNormal_EnInterurbano_Cobra3000()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto = colectivoInterurbano.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(TARIFA_INTERURBANA, boleto.MontoPagado,
                "Debe cobrar $3000 en colectivo interurbano");
            Assert.AreEqual(5000m - TARIFA_INTERURBANA, tarjeta.Saldo);
        }

        [Test]
        public void Test_TarjetaNormal_EnUrbano_Cobra1580()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto = colectivoUrbano.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(TARIFA_URBANA, boleto.MontoPagado,
                "Debe cobrar $1580 en colectivo urbano");
        }

        [Test]
        public void Test_DiferenciaEntreUrbanoEInterurbano()
        {
            Tarjeta tarjeta1 = new Tarjeta(tiempo);
            Tarjeta tarjeta2 = new Tarjeta(tiempo);
            
            tarjeta1.Cargar(5000m);
            tarjeta2.Cargar(5000m);

            Boleto boletoUrbano = colectivoUrbano.PagarCon(tarjeta1);
            Boleto boletoInterurbano = colectivoInterurbano.PagarCon(tarjeta2);

            Assert.AreEqual(TARIFA_URBANA, boletoUrbano.MontoPagado);
            Assert.AreEqual(TARIFA_INTERURBANA, boletoInterurbano.MontoPagado);
            Assert.Greater(boletoInterurbano.MontoPagado, boletoUrbano.MontoPagado,
                "Tarifa interurbana debe ser mayor");
        }

        #endregion

        #region Tests con Franquicias en Interurbano

        [Test]
        public void Test_MedioBoleto_EnInterurbano_DentroDeHorario()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto = colectivoInterurbano.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(TARIFA_INTERURBANA * 0.5m, boleto.MontoPagado,
                "Medio boleto debe aplicar 50% descuento sobre tarifa interurbana ($1500)");
        }

        [Test]
        public void Test_FranquiciaCompleta_EnInterurbano_PrimerosViajesGratis()
        {
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta(tiempo);

            Boleto boleto1 = colectivoInterurbano.PagarCon(tarjeta);
            Boleto boleto2 = colectivoInterurbano.PagarCon(tarjeta);

            Assert.IsNotNull(boleto1);
            Assert.IsNotNull(boleto2);
            Assert.AreEqual(0m, boleto1.MontoPagado, "Primer viaje debe ser gratis");
            Assert.AreEqual(0m, boleto2.MontoPagado, "Segundo viaje debe ser gratis");
        }

        [Test]
        public void Test_FranquiciaCompleta_EnInterurbano_TercerViajeCobrado()
        {
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta(tiempo);
            tarjeta.Cargar(5000m);

            colectivoInterurbano.PagarCon(tarjeta); // Viaje 1 - gratis
            colectivoInterurbano.PagarCon(tarjeta); // Viaje 2 - gratis
            Boleto boleto3 = colectivoInterurbano.PagarCon(tarjeta); // Viaje 3 - cobrado

            Assert.IsNotNull(boleto3);
            Assert.AreEqual(TARIFA_INTERURBANA, boleto3.MontoPagado,
                "Tercer viaje debe cobrar tarifa interurbana completa ($3000)");
        }

        [Test]
        public void Test_MedioBoleto_FueraDeHorario_EnInterurbano_Rechaza()
        {
            // Cambiar a sábado
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 23, 10, 0, 0));
            
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto = colectivoInterurbano.PagarCon(tarjeta);

            Assert.IsNull(boleto,
                "No debe permitir viaje fuera de horario en interurbano");
        }

        [Test]
        public void Test_FranquiciaCompleta_FueraDeHorario_EnInterurbano_Rechaza()
        {
            // Cambiar a domingo
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 24, 10, 0, 0));
            
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta(tiempo);

            Boleto boleto = colectivoInterurbano.PagarCon(tarjeta);

            Assert.IsNull(boleto,
                "No debe permitir viaje fuera de horario en interurbano");
        }

        #endregion

        #region Tests de Saldo Insuficiente en Interurbano

        [Test]
        public void Test_ConSaldoNegativo_EnInterurbano_Permite()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(2000m); // 2000 - 3000 = -1000 (dentro del límite -1200)

            Boleto boleto = colectivoInterurbano.PagarCon(tarjeta);

            Assert.IsNotNull(boleto, "Debe permitir viaje con saldo negativo");
            Assert.AreEqual(-1000m, tarjeta.Saldo);
        }

        [Test]
        public void Test_SuperaLimiteSaldoNegativo_EnInterurbano_Rechaza()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(2000m);

            colectivoInterurbano.PagarCon(tarjeta); // Primer viaje: 2000 - 3000 = -1000
            Boleto boleto2 = colectivoInterurbano.PagarCon(tarjeta); // -1000 - 3000 = -4000 (supera límite)

            Assert.IsNull(boleto2,
                "No debe permitir si supera el límite de saldo negativo (-1200)");
        }

        #endregion

        #region Tests de Boleto Generado

        [Test]
        public void Test_BoletoInterurbano_ContieneInformacionCorrecta()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(5000m);
            int idTarjeta = tarjeta.Id;

            Boleto boleto = colectivoInterurbano.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(TARIFA_INTERURBANA, boleto.MontoPagado);
            Assert.AreEqual("500", boleto.LineaColectivo);
            Assert.AreEqual("Galvez Express", boleto.Empresa);
            Assert.AreEqual(2000m, boleto.SaldoRestante);
            Assert.AreEqual("Normal", boleto.TipoTarjeta);
            Assert.AreEqual(idTarjeta, boleto.IdTarjeta);
            Assert.AreEqual(new DateTime(2024, 11, 25, 10, 0, 0), boleto.FechaHora);
        }

        [Test]
        public void Test_MultiplesViajesInterurbanos_DescuentanCorrectamente()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(10000m);

            Boleto b1 = colectivoInterurbano.PagarCon(tarjeta); // 10000 - 3000 = 7000
            Boleto b2 = colectivoInterurbano.PagarCon(tarjeta); // 7000 - 3000 = 4000
            Boleto b3 = colectivoInterurbano.PagarCon(tarjeta); // 4000 - 3000 = 1000

            Assert.IsNotNull(b1);
            Assert.IsNotNull(b2);
            Assert.IsNotNull(b3);
            
            Assert.AreEqual(7000m, b1.SaldoRestante);
            Assert.AreEqual(4000m, b2.SaldoRestante);
            Assert.AreEqual(1000m, b3.SaldoRestante);
            Assert.AreEqual(1000m, tarjeta.Saldo);
        }

        #endregion

        #region Tests de Uso Frecuente en Interurbano

        [Test]
        public void Test_UsoFrecuente_NoAplicaEnInterurbano()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            for (int i = 0; i < 2; i++)
            {
                tarjeta.Cargar(30000m);
            }

            // Realizar 35 viajes interurbanos (en urbano tendría 20% descuento)
            for (int i = 0; i < 35; i++)
            {
                Boleto boleto = colectivoInterurbano.PagarCon(tarjeta);
                if (boleto != null)
                {
                    Assert.AreEqual(TARIFA_INTERURBANA, boleto.MontoPagado,
                        $"Viaje {i + 1}: Interurbano siempre debe cobrar $3000 (sin descuento por uso frecuente)");
                }
            }
        }

        #endregion

        #region Tests de Propiedades del Colectivo

        [Test]
        public void Test_PropiedadesColectivoInterurbano()
        {
            Assert.AreEqual("500", colectivoInterurbano.Linea);
            Assert.AreEqual("Galvez Express", colectivoInterurbano.Empresa);
        }

        [Test]
        public void Test_DiferentesLineasInterurbanas()
        {
            ColectivoInterurbano galvez = new ColectivoInterurbano("500", "Galvez Express", tiempo);
            ColectivoInterurbano baigorria = new ColectivoInterurbano("501", "Baigorria Trans", tiempo);

            Assert.AreEqual("500", galvez.Linea);
            Assert.AreEqual("501", baigorria.Linea);
            Assert.AreNotEqual(galvez.Linea, baigorria.Linea);
        }

        #endregion

        #region Tests de Horarios Especiales

        [Test]
        public void Test_TarjetaNormal_FuncionaCualquierHorario_EnInterurbano()
        {
            // Sábado
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 23, 10, 0, 0));
            Tarjeta tarjeta1 = new Tarjeta(tiempo);
            tarjeta1.Cargar(5000);
            Boleto boleto1 = colectivoInterurbano.PagarCon(tarjeta1);
            Assert.IsNotNull(boleto1);
            Assert.AreEqual(TARIFA_INTERURBANA, boleto1.MontoPagado);

            // Domingo madrugada
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 24, 3, 0, 0));
            Tarjeta tarjeta2 = new Tarjeta(tiempo);
            tarjeta2.Cargar(5000);
            Boleto boleto2 = colectivoInterurbano.PagarCon(tarjeta2);
            Assert.IsNotNull(boleto2);
            Assert.AreEqual(TARIFA_INTERURBANA, boleto2.MontoPagado);
        }

        [Test]
        public void Test_MedioBoleto_Lunes6AM_EnInterurbano_Funciona()
        {
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 6, 0, 0));
            
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000);

            Boleto boleto = colectivoInterurbano.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(TARIFA_INTERURBANA * 0.5m, boleto.MontoPagado);
        }

        [Test]
        public void Test_MedioBoleto_Lunes22PM_EnInterurbano_NoFunciona()
        {
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 22, 0, 0));
            
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000);

            Boleto boleto = colectivoInterurbano.PagarCon(tarjeta);

            Assert.IsNull(boleto, "Franquicia no debe funcionar a las 22:00");
        }

        #endregion

        #region Tests de Comparación Urbano vs Interurbano

        [Test]
        public void Test_MismaTarjeta_DiferenteTarifa_UrbanVsInterurbano()
        {
            Tarjeta tarjeta = new Tarjeta(tiempo);
            tarjeta.Cargar(10000);

            Boleto boletoUrbano = colectivoUrbano.PagarCon(tarjeta);
            tiempo.AvanzarMinutos(65); // Esperar para segundo viaje
            Boleto boletoInterurbano = colectivoInterurbano.PagarCon(tarjeta);

            Assert.AreEqual(TARIFA_URBANA, boletoUrbano.MontoPagado);
            Assert.AreEqual(TARIFA_INTERURBANA, boletoInterurbano.MontoPagado);
            Assert.AreEqual(10000 - TARIFA_URBANA - TARIFA_INTERURBANA, tarjeta.Saldo);
        }

        [Test]
        public void Test_MedioBoleto_DiferenteTarifa_UrbanoVsInterurbano()
        {
            TarjetaMedioBoleto tarjetaUrbano = new TarjetaMedioBoleto(tiempo);
            tarjetaUrbano.Cargar(10000);

            TarjetaMedioBoleto tarjetaInterUrbano = new TarjetaMedioBoleto(tiempo);
            tarjetaInterUrbano.Cargar(10000);

            Boleto boletoUrbano = colectivoUrbano.PagarCon(tarjetaUrbano);
            tiempo.AvanzarMinutos(6); // Esperar para segundo viaje
            Boleto boletoInterurbano = colectivoInterurbano.PagarCon(tarjetaInterUrbano);

            Assert.AreEqual(TARIFA_URBANA * 0.5m, boletoUrbano.MontoPagado); // 790
            Assert.AreEqual(TARIFA_INTERURBANA * 0.5m, boletoInterurbano.MontoPagado); // 1500
        }

        #endregion
    }
}*/
