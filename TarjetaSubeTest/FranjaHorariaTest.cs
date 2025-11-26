using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class FranjaHorariaTest
    {
        private TiempoSimulado tiempo;
        private Colectivo colectivo;

        [SetUp]
        public void Setup()
        {
            // Inicializar en lunes 25 nov 2024, 10:00 AM (horario permitido)
            tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            colectivo = new Colectivo("152", "Rosario Bus", tiempo);
        }

        [Test]
        public void Test_MedioBoleto_LunesDentroDeHorario_Acepta()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto, 
                "Debe permitir viaje de lunes a viernes entre 6 y 22 horas");
            Assert.AreEqual(790m, boleto.MontoPagado);
        }

        [Test]
        public void Test_MedioBoleto_MartesDentroDeHorario_Acepta()
        {
            // Martes 26 nov 2024, 14:30
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 26, 14, 30, 0));
            
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(790m, boleto.MontoPagado);
        }

        [Test]
        public void Test_MedioBoleto_ViernesDentroDeHorario_Acepta()
        {
            // Viernes 29 nov 2024, 18:00
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 29, 18, 0, 0));
            
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(790m, boleto.MontoPagado);
        }

        [Test]
        public void Test_MedioBoleto_LunesA6AM_Acepta()
        {
            // Lunes 25 nov 2024, 6:00 AM (límite inferior)
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 6, 0, 0));
            
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto, "Debe aceptar a las 6:00 AM");
        }

        [Test]
        public void Test_MedioBoleto_LunesA21_59_Acepta()
        {
            // Lunes 25 nov 2024, 21:59 (antes del límite superior)
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 21, 59, 0));
            
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto, "Debe aceptar hasta las 21:59");
        }

        [Test]
        public void Test_MedioBoleto_LunesA22_00_Rechaza()
        {
            // Lunes 25 nov 2024, 22:00 (límite superior excluido)
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 22, 0, 0));
            
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNull(boleto, "No debe permitir a partir de las 22:00");
        }

        [Test]
        public void Test_MedioBoleto_LunesA5_59_Rechaza()
        {
            // Lunes 25 nov 2024, 5:59 AM
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 5, 59, 0));
            
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNull(boleto, "No debe permitir antes de las 6:00");
        }

        [Test]
        public void Test_MedioBoleto_Sabado_Rechaza()
        {
            // Sábado 23 nov 2024, 10:00 AM
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 23, 10, 0, 0));
            
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNull(boleto, "No debe permitir viaje en sábado");
        }

        [Test]
        public void Test_MedioBoleto_Domingo_Rechaza()
        {
            // Domingo 24 nov 2024, 10:00 AM
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 24, 10, 0, 0));
            
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNull(boleto, "No debe permitir viaje en domingo");
        }

        [Test]
        public void Test_MedioBoleto_LunesMedianoche_Rechaza()
        {
            // Lunes 25 nov 2024, 00:00 (medianoche)
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 0, 0, 0));
            
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNull(boleto, "No debe permitir viaje a medianoche");
        }

        [Test]
        public void Test_FranquiciaCompleta_LunesDentroDeHorario_Acepta()
        {
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta(tiempo);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(0m, boleto.MontoPagado);
        }

        [Test]
        public void Test_FranquiciaCompleta_ViernesDentroDeHorario_Acepta()
        {
            // Viernes 29 nov 2024, 15:00
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 29, 15, 0, 0));
            
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta(tiempo);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
        }

        [Test]
        public void Test_FranquiciaCompleta_Sabado_Rechaza()
        {
            // Sábado 23 nov 2024, 10:00
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 23, 10, 0, 0));
            
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta(tiempo);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNull(boleto, "No debe permitir viaje en sábado");
        }

        [Test]
        public void Test_FranquiciaCompleta_Domingo_Rechaza()
        {
            // Domingo 24 nov 2024, 10:00
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 24, 10, 0, 0));
            
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta(tiempo);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNull(boleto, "No debe permitir viaje en domingo");
        }

        [Test]
        public void Test_FranquiciaCompleta_LunesAntesDe6AM_Rechaza()
        {
            // Lunes 25 nov 2024, 5:00 AM
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 5, 0, 0));
            
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta(tiempo);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNull(boleto, "No debe permitir antes de las 6:00");
        }

        [Test]
        public void Test_FranquiciaCompleta_LunesDespuesDe22_Rechaza()
        {
            // Lunes 25 nov 2024, 23:00
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 23, 0, 0));
            
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta(tiempo);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNull(boleto, "No debe permitir después de las 22:00");
        }

        [Test]
        public void Test_TarjetaNormal_NoTieneRestriccionHoraria()
        {
            // Probar en distintos horarios
            
            // Sábado 10:00
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 23, 10, 0, 0));
            Tarjeta tarjeta1 = new Tarjeta(tiempo);
            tarjeta1.Cargar(5000m);
            Boleto boleto1 = colectivo.PagarCon(tarjeta1);
            Assert.IsNotNull(boleto1, "Tarjeta normal funciona sábado");

            // Domingo 3:00 AM
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 24, 3, 0, 0));
            Tarjeta tarjeta2 = new Tarjeta(tiempo);
            tarjeta2.Cargar(5000m);
            Boleto boleto2 = colectivo.PagarCon(tarjeta2);
            Assert.IsNotNull(boleto2, "Tarjeta normal funciona domingo madrugada");

            // Lunes 23:00
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 23, 0, 0));
            Tarjeta tarjeta3 = new Tarjeta(tiempo);
            tarjeta3.Cargar(5000m);
            Boleto boleto3 = colectivo.PagarCon(tarjeta3);
            Assert.IsNotNull(boleto3, "Tarjeta normal funciona lunes noche");
        }

        [Test]
        public void Test_BoletoEstudiantil_SigueMismasReglas()
        {
            // Dentro de horario - acepta
            TarjetaBoletoGratuitoEstudiantil tarjeta1 = new TarjetaBoletoGratuitoEstudiantil(tiempo);
            Boleto boleto1 = colectivo.PagarCon(tarjeta1);
            Assert.IsNotNull(boleto1);

            // Sábado - rechaza
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 23, 10, 0, 0));
            TarjetaBoletoGratuitoEstudiantil tarjeta2 = new TarjetaBoletoGratuitoEstudiantil(tiempo);
            Boleto boleto2 = colectivo.PagarCon(tarjeta2);
            Assert.IsNull(boleto2);

            // Fuera de horario - rechaza
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 23, 0, 0));
            TarjetaBoletoGratuitoEstudiantil tarjeta3 = new TarjetaBoletoGratuitoEstudiantil(tiempo);
            Boleto boleto3 = colectivo.PagarCon(tarjeta3);
            Assert.IsNull(boleto3);
        }

        [Test]
        public void Test_TodasLasFranquicias_FranjaHoraria()
        {
            // Este test verifica que todas las franquicias respetan la franja horaria

            // Lunes 10:00 - todos aceptan
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 10, 0, 0));
            
            TarjetaMedioBoleto medio = new TarjetaMedioBoleto(tiempo);
            medio.Cargar(5000m);
            Assert.IsNotNull(colectivo.PagarCon(medio), "Medio boleto acepta L-V 6-22");

            TarjetaFranquiciaCompleta franquicia = new TarjetaFranquiciaCompleta(tiempo);
            Assert.IsNotNull(colectivo.PagarCon(franquicia), "Franquicia completa acepta L-V 6-22");

            TarjetaBoletoGratuitoEstudiantil estudiantil = new TarjetaBoletoGratuitoEstudiantil(tiempo);
            Assert.IsNotNull(colectivo.PagarCon(estudiantil), "Boleto estudiantil acepta L-V 6-22");

            // Sábado 10:00 - todos rechazan
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 23, 10, 0, 0));
            
            TarjetaMedioBoleto medio2 = new TarjetaMedioBoleto(tiempo);
            medio2.Cargar(5000m);
            Assert.IsNull(colectivo.PagarCon(medio2), "Medio boleto rechaza sábado");

            TarjetaFranquiciaCompleta franquicia2 = new TarjetaFranquiciaCompleta(tiempo);
            Assert.IsNull(colectivo.PagarCon(franquicia2), "Franquicia completa rechaza sábado");

            TarjetaBoletoGratuitoEstudiantil estudiantil2 = new TarjetaBoletoGratuitoEstudiantil(tiempo);
            Assert.IsNull(colectivo.PagarCon(estudiantil2), "Boleto estudiantil rechaza sábado");
        }

        [Test]
        public void Test_TransicionDeHorario_6AM()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            // 5:59 AM - rechaza
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 5, 59, 0));
            Assert.IsNull(colectivo.PagarCon(tarjeta), "Rechaza a las 5:59");

            // 6:00 AM - acepta
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 6, 0, 0));
            Assert.IsNotNull(colectivo.PagarCon(tarjeta), "Acepta a las 6:00");
        }

        [Test]
        public void Test_TransicionDeHorario_22PM()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto(tiempo);
            tarjeta.Cargar(5000m);

            // 21:59 PM - acepta
            tiempo.EstablecerTiempo(new DateTime(2024, 11, 25, 21, 59, 0));
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1, "Acepta a las 21:59");

            // Esperar 6 minutos para el segundo viaje
            tiempo.AvanzarMinutos(6);

            // 22:00 PM - rechaza (ahora está fuera de horario)
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.IsNull(boleto2, "Rechaza a partir de las 22:00");
        }
    }
}