using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class SaldoNegativoTest
    {
        private Colectivo colectivo;
        private const decimal TARIFA_BASICA = 1580m;
        private const decimal SALDO_NEGATIVO_PERMITIDO = 1200m;

        [SetUp]
        public void Setup()
        {
            colectivo = new Colectivo("152", "Rosario Bus");
        }

        #region Tests de Saldo Negativo Básico

        [Test]
        public void Test_TarjetaSinSaldo_PuedeViajarConSaldoNegativo()
        {
            Tarjeta tarjeta = new Tarjeta();
            // Cargar poco saldo para que al viajar quede negativo
            tarjeta.Cargar(2000m); // Saldo = 2000

            // Primer viaje: 2000 - 1580 = 420
            tarjeta.PagarPasaje();
            Assert.AreEqual(420m, tarjeta.Saldo);

            // Segundo viaje con saldo negativo: 420 - 1580 = -1160
            bool resultado = tarjeta.PagarPasaje();

            Assert.IsTrue(resultado, "Debe permitir viajar quedando con saldo negativo");
            Assert.AreEqual(-1160m, tarjeta.Saldo, "El saldo debe ser -1160");
        }

        [Test]
        public void Test_SaldoNegativo_NoSuperaLimitePermitido()
        {
            Tarjeta tarjeta = new Tarjeta();
            
            // Intentar descontar más del límite negativo permitido
            bool resultado = tarjeta.Descontar(SALDO_NEGATIVO_PERMITIDO + 1);

            Assert.IsFalse(resultado, "No debe permitir superar el límite de saldo negativo");
            Assert.AreEqual(0m, tarjeta.Saldo, "El saldo debe permanecer en 0");
        }

        [Test]
        public void Test_SaldoNegativo_ExactamenteEnElLimite()
        {
            Tarjeta tarjeta = new Tarjeta();
            
            // Descontar exactamente el límite negativo permitido
            bool resultado = tarjeta.Descontar(SALDO_NEGATIVO_PERMITIDO);

            Assert.IsTrue(resultado, "Debe permitir llegar exactamente al límite negativo");
            Assert.AreEqual(-SALDO_NEGATIVO_PERMITIDO, tarjeta.Saldo, 
                $"El saldo debe ser -{SALDO_NEGATIVO_PERMITIDO}");
        }

        [Test]
        public void Test_SaldoNegativo_NoPermiteSuperarLimite()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Descontar(SALDO_NEGATIVO_PERMITIDO); // Saldo = -1200

            // Intentar otro viaje que superaría el límite
            bool resultado = tarjeta.PagarPasaje();

            Assert.IsFalse(resultado, "No debe permitir viaje que supere límite negativo");
            Assert.AreEqual(-SALDO_NEGATIVO_PERMITIDO, tarjeta.Saldo, 
                "El saldo debe permanecer en el límite");
        }

        #endregion

        #region Tests de Viajes Plus

        [Test]
        public void Test_ViajeConSaldoInsuficiente_UsaSaldoNegativo()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1500m); // Saldo = 500 (menos que TARIFA_BASICA)

            // Debe permitir viajar usando saldo negativo
            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto, "Debe generar boleto aunque saldo sea insuficiente");
            Assert.AreEqual(500m - TARIFA_BASICA, tarjeta.Saldo, 
                "El saldo debe quedar negativo");
            Assert.Less(tarjeta.Saldo, 0, "El saldo debe ser negativo");
        }

        [Test]
        public void Test_BoletoConSaldoNegativo_MuestraMontoCorrecto()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1500m); // Saldo = 500

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.AreEqual(TARIFA_BASICA, boleto.MontoPagado, 
                "El monto pagado debe ser la tarifa completa");
            Assert.AreEqual(500m - TARIFA_BASICA, boleto.SaldoRestante, 
                "El saldo restante debe mostrar el saldo negativo");
            Assert.Less(boleto.SaldoRestante, 0, "El saldo restante debe ser negativo");
        }

        [Test]
        public void Test_UnViajePlus_SaldoNegativoHasta1200()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1600m); // Saldo = 400

            bool resultado = tarjeta.PagarPasaje(); // Intenta descontar 1580

            Assert.IsTrue(resultado, "Debe permitir un viaje plus");
            Assert.AreEqual(400m - TARIFA_BASICA, tarjeta.Saldo, 
                "El saldo debe ser 400 - 1580 = -1180");
            Assert.Greater(tarjeta.Saldo, -SALDO_NEGATIVO_PERMITIDO, 
                "El saldo no debe superar el límite negativo");
        }

        [Test]
        public void Test_NoPermiteDosViajesPlus_SiSuperaLimite()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1600m); // Saldo = 400

            tarjeta.PagarPasaje(); // Saldo = 400 - 1580 = -1180
            
            // Intentar otro viaje que llevaría el saldo a -2760
            bool resultado = tarjeta.PagarPasaje();

            Assert.IsFalse(resultado, 
                "No debe permitir segundo viaje plus que supere -1200");
            Assert.AreEqual(-1180m, tarjeta.Saldo, 
                "El saldo debe permanecer en -1180");
        }

        #endregion

        #region Tests de Recarga con Saldo Negativo

        [Test]
        public void Test_RecargaConSaldoNegativo_DescuentaDeudaPrimero()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Descontar(800m); // Saldo = -800

            // Cargar 2000, debe primero pagar la deuda
            tarjeta.Cargar(2000m);

            Assert.AreEqual(1200m, tarjeta.Saldo, 
                "Debe descontar primero la deuda: -800 + 2000 = 1200");
        }

        [Test]
        public void Test_RecargaConSaldoNegativo_CargaMasQueDeuda() // la deuda maxima es de 1200 entonces no se puede cargar y seguir quedando en negativo, pq el monto minimo de carga es 2000
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Descontar(1000m); // Saldo = -1000

            // Cargar 2000, pero la deuda es 1000
            tarjeta.Cargar(2000m);

            Assert.AreEqual(1000m, tarjeta.Saldo, 
                "Debe quedar: -1000 + 2000 = 1000");
        }

        [Test]
        public void Test_VariosViajesPlus_YRecarga()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(3000m); // Saldo = 3000
            
            // Primer viaje normal
            tarjeta.PagarPasaje(); // Saldo = 1420
            
            // Segundo viaje con saldo insuficiente (viaje plus)
            tarjeta.PagarPasaje(); // Saldo = 1420 - 1580 = -160
            
            Assert.AreEqual(-160m, tarjeta.Saldo, "Debe quedar con saldo negativo");
            
            // Recargar
            tarjeta.Cargar(2000m);
            
            Assert.AreEqual(1840m, tarjeta.Saldo, 
                "Debe descontar la deuda: -160 + 2000 = 1840");
        }

        #endregion

        #region Tests de Integración con Colectivo

        [Test]
        public void Test_ColectivoPagarCon_TarjetaConSaldoNegativo()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1600m); // Saldo = 400

            // Primer viaje con saldo insuficiente
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1, "Debe generar boleto con viaje plus");
            Assert.AreEqual(-1180m, tarjeta.Saldo);

            // Segundo viaje debe fallar (superaría límite)
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.IsNull(boleto2, "No debe generar boleto si supera límite negativo");
        }

        [Test]
        public void Test_MultiplesViajes_ConYSinSaldoNegativo()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000m);
            tarjeta.Cargar(2000m); // hago por separado pq 7000 no es monto valido 

            // Viajes normales hasta quedarse con poco saldo
            Boleto b1 = colectivo.PagarCon(tarjeta); // 7000 - 1580 = 5420
            Assert.IsNotNull(b1);
            Assert.AreEqual(5420m, tarjeta.Saldo);
            
            Boleto b2 = colectivo.PagarCon(tarjeta); // 5420 - 1580 = 3840
            Assert.IsNotNull(b2);
            Assert.AreEqual(3840m, tarjeta.Saldo);
            
            Boleto b3 = colectivo.PagarCon(tarjeta); // 3840 - 1580 = 2260
            Assert.IsNotNull(b3);
            Assert.AreEqual(2260m, tarjeta.Saldo);

            Boleto b4 = colectivo.PagarCon(tarjeta); // 2260 - 1580 = 680
            Assert.IsNotNull(b4); 
            Assert.AreEqual(680m, tarjeta.Saldo);

            Boleto boletoPlus = colectivo.PagarCon(tarjeta); // 680 - 1580 = -900
            Assert.IsNotNull(boletoPlus, "debe permitir un viaje plus, con saldo negativo"); 
            Assert.AreEqual(-900m, tarjeta.Saldo);

            // Recargar
            tarjeta.Cargar(3000m);
            Assert.AreEqual(2100m, tarjeta.Saldo, "Debe descontar deuda: -900 + 3000 = 2100");

            // Viaje normal después de recargar: 2100 - 1580 = 520
            Boleto boletoNormal = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boletoNormal);
            Assert.AreEqual(520m, tarjeta.Saldo);
        }

        #endregion

        #region Tests de Casos Límite

        [Test]
        public void Test_SaldoExactamente_Menos1200()
        {
            Tarjeta tarjeta = new Tarjeta();
            
            bool resultado = tarjeta.Descontar(1200m);
            
            Assert.IsTrue(resultado, "Debe permitir saldo de exactamente -1200");
            Assert.AreEqual(-1200m, tarjeta.Saldo);
        }

        [Test]
        public void Test_SaldoMenos1201_NoPermitido()
        {
            Tarjeta tarjeta = new Tarjeta();
            
            bool resultado = tarjeta.Descontar(1201m);
            
            Assert.IsFalse(resultado, "No debe permitir saldo menor a -1200");
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void Test_DesdeNegativo_RecargarHastaPositivo()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Descontar(500m); // -500
            
            tarjeta.Cargar(3000m); // -500 + 3000 = 2500
            
            Assert.AreEqual(2500m, tarjeta.Saldo);
        }

        [Test]
        public void Test_DesdeNegativo_RecargarYSuperarLimite()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000m); // Cargar primero
            tarjeta.Descontar(3000m); // 2000 - 3000 = -1000
            
            Assert.AreEqual(-1000m, tarjeta.Saldo, "Saldo debe ser -1000");
            
            // Recargar hasta cerca del límite
            tarjeta.Cargar(30000m); // -1000 + 30000 = 29000
            Assert.AreEqual(29000m, tarjeta.Saldo);
            
            tarjeta.Cargar(30000m); // 29000 + 30000 = 59000, pero límite es 56000
            
            Assert.AreEqual(56000m, tarjeta.Saldo, 
                "Debe respetar el límite de 56000");
            Assert.AreEqual(3000m, tarjeta.SaldoPendiente, 
                "El excedente debe ir a pendiente: 59000 - 56000 = 3000");
        }

        [Test]
        public void Test_SaldoNegativo_ConSaldoPendiente()
        {
            Tarjeta tarjeta = new Tarjeta();
            
            // Llenar hasta el límite con pendiente
            tarjeta.Cargar(30000m);
            tarjeta.Cargar(30000m); // Saldo: 56000, Pendiente: 4000
            
            // Hacer varios viajes hasta tener poco saldo
            for (int i = 0; i < 35; i++)
            {
                tarjeta.PagarPasaje();
            }
            
            // El saldo debe irse recargando automáticamente del pendiente
            // Después de 35 viajes: 35 * 1580 = 55300
            // Como el saldo se recarga automáticamente, debe funcionar correctamente
            
            Assert.Greater(tarjeta.Saldo, 0m, 
                "El saldo debe mantenerse positivo gracias al saldo pendiente");
        }

        [Test]
        public void Test_ViajePlus_BoletoCorrecto()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1500m); // Saldo = 500
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual("152", boleto.LineaColectivo);
            Assert.AreEqual("Rosario Bus", boleto.Empresa);
            Assert.AreEqual(TARIFA_BASICA, boleto.MontoPagado);
            Assert.AreEqual(-1080m, boleto.SaldoRestante);
            Assert.AreEqual("Normal", boleto.TipoTarjeta);
            Assert.IsNotNull(boleto.FechaHora);
        }

        #endregion
    }
}
