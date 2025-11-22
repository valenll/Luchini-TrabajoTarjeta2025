using NUnit.Framework;
using System;

namespace TarjetaTests
{
    [TestFixture]
    public class TarjetaTests
    {
        private const decimal LIMITE_SALDO = 56000m;
        private const decimal TARIFA_BASICA = 1580m;

        #region Tests Principales Requeridos

        [Test]
        public void Test_CargaSuperaMaximo_AcreditaHastaLimiteYGuardaExcedente()
        {
            Tarjeta tarjeta = new Tarjeta();
            
            // Cargar hasta estar cerca del límite (54000)
            tarjeta.Cargar(30000);
            tarjeta.Cargar(20000);
            tarjeta.Cargar(4000);
            
            // Verificar que tenemos 54000 en saldo
            Assert.AreEqual(54000m, tarjeta.Saldo, "El saldo inicial debería ser 54000");
            Assert.AreEqual(0m, tarjeta.SaldoPendiente, "No debería haber saldo pendiente inicialmente");

            // Intentar cargar 10000 más (superaría el límite en 8000)
            bool resultado = tarjeta.Cargar(10000);

            Assert.IsTrue(resultado, "La carga debería ser exitosa");
            Assert.AreEqual(LIMITE_SALDO, tarjeta.Saldo, "El saldo debería estar en el límite máximo de 56000");
            Assert.AreEqual(8000m, tarjeta.SaldoPendiente, "Deberían quedar 8000 pendientes de acreditación");
        }

        [Test]
        public void Test_LuegoDeViaje_AcreditaSaldoPendienteAutomaticamente()
        {
            Tarjeta tarjeta = new Tarjeta();
            
            // Cargar hasta superar el límite
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); // Saldo = 56000, Pendiente = 4000
            
            Assert.AreEqual(LIMITE_SALDO, tarjeta.Saldo, "Saldo inicial debería ser 56000");
            Assert.AreEqual(4000m, tarjeta.SaldoPendiente, "Saldo pendiente inicial debería ser 4000");

            // Realizar un viaje (descuenta TARIFA_BASICA = 1580)
            bool viajeExitoso = tarjeta.PagarPasaje();

            Assert.IsTrue(viajeExitoso, "El viaje debería ser exitoso");
            
            // Después del viaje: 56000 - 1580 = 54420
            // Se acreditan los 4000 pendientes, pero solo cabe 1580 para llegar a 56000
            decimal saldoEsperado = LIMITE_SALDO;
            decimal pendienteEsperado = 4000m - 1580m; // 2420
            
            Assert.AreEqual(saldoEsperado, tarjeta.Saldo, 
                "Después del viaje, el saldo debería recargarse hasta el máximo (56000)");
            Assert.AreEqual(pendienteEsperado, tarjeta.SaldoPendiente, 
                "Deberían quedar 2420 pendientes de acreditación");
        }

        #endregion

        #region Tests de Cobertura Completa de Tarjeta.cs

        [Test]
        public void Test_Constructor_InicializaCorrectamente()
        {
            Tarjeta tarjeta = new Tarjeta();

            Assert.AreEqual(0m, tarjeta.Saldo, "El saldo inicial debe ser 0");
            Assert.AreEqual(0m, tarjeta.SaldoPendiente, "El saldo pendiente inicial debe ser 0");
            Assert.Greater(tarjeta.Id, 0, "El ID debe ser mayor a 0");
        }

        [Test]
        public void Test_PropertySaldo_RetornaValorCorrecto()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            decimal saldo = tarjeta.Saldo;

            Assert.AreEqual(5000m, saldo);
        }

        [Test]
        public void Test_PropertySaldoPendiente_RetornaValorCorrecto()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);

            decimal pendiente = tarjeta.SaldoPendiente;

            Assert.AreEqual(4000m, pendiente);
        }

        [Test]
        public void Test_PropertyId_EsUnico()
        {
            Tarjeta tarjeta1 = new Tarjeta();
            Tarjeta tarjeta2 = new Tarjeta();
            Tarjeta tarjeta3 = new Tarjeta();

            Assert.AreNotEqual(tarjeta1.Id, tarjeta2.Id, "Los IDs deben ser únicos");
            Assert.AreNotEqual(tarjeta2.Id, tarjeta3.Id, "Los IDs deben ser únicos");
            Assert.AreNotEqual(tarjeta1.Id, tarjeta3.Id, "Los IDs deben ser únicos");
        }

        [Test]
        [TestCase(2000)]
        [TestCase(3000)]
        [TestCase(4000)]
        [TestCase(5000)]
        [TestCase(8000)]
        [TestCase(10000)]
        [TestCase(15000)]
        [TestCase(20000)]
        [TestCase(25000)]
        [TestCase(30000)]
        public void Test_Cargar_MontosValidos_RetornaTrue(decimal monto)
        {
            Tarjeta tarjeta = new Tarjeta();

            bool resultado = tarjeta.Cargar(monto);

            Assert.IsTrue(resultado, $"Debería aceptar el monto {monto}");
            Assert.AreEqual(monto, tarjeta.Saldo, $"El saldo debería ser {monto}");
        }

        [Test]
        [TestCase(1000)]
        [TestCase(1500)]
        [TestCase(2500)]
        [TestCase(7000)]
        [TestCase(12000)]
        [TestCase(35000)]
        [TestCase(100)]
        [TestCase(50000)]
        public void Test_Cargar_MontosInvalidos_RetornaFalse(decimal monto)
        {
            Tarjeta tarjeta = new Tarjeta();

            bool resultado = tarjeta.Cargar(monto);

            Assert.IsFalse(resultado, $"No debería aceptar el monto {monto}");
            Assert.AreEqual(0m, tarjeta.Saldo, "El saldo debería permanecer en 0");
        }

        [Test]
        public void Test_Cargar_SinExcederLimite_AcreditaTodoAlSaldo()
        {
            Tarjeta tarjeta = new Tarjeta();

            tarjeta.Cargar(20000);
            tarjeta.Cargar(15000);

            Assert.AreEqual(35000m, tarjeta.Saldo);
            Assert.AreEqual(0m, tarjeta.SaldoPendiente);
        }

        [Test]
        public void Test_Cargar_ExactamenteElLimite_NoGeneraPendiente()
        {
            Tarjeta tarjeta = new Tarjeta();

            tarjeta.Cargar(30000);
            tarjeta.Cargar(20000);
            tarjeta.Cargar(5000);
            tarjeta.Cargar(2000); // Total: 57000, pero solo cabe 56000

            Assert.AreEqual(LIMITE_SALDO, tarjeta.Saldo);
            Assert.AreEqual(1000m, tarjeta.SaldoPendiente);
        }

        [Test]
        public void Test_Cargar_SuperaLimitePorMucho_GuardaTodoElExcedente()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);

            tarjeta.Cargar(30000); // Total: 60000, excede por 4000

            Assert.AreEqual(LIMITE_SALDO, tarjeta.Saldo);
            Assert.AreEqual(4000m, tarjeta.SaldoPendiente);
        }

        [Test]
        public void Test_Cargar_EnElLimite_TodoVaAPendiente()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);
            tarjeta.Cargar(20000);
            tarjeta.Cargar(5000);
            tarjeta.Cargar(2000); // Saldo: 56000, Pendiente: 1000

            tarjeta.Cargar(10000); // Todo debe ir a pendiente

            Assert.AreEqual(LIMITE_SALDO, tarjeta.Saldo);
            Assert.AreEqual(11000m, tarjeta.SaldoPendiente);
        }

        [Test]
        public void Test_AcreditarCarga_SinSaldoPendiente_NoHaceNada()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(10000);
            decimal saldoInicial = tarjeta.Saldo;

            tarjeta.AcreditarCarga();

            Assert.AreEqual(saldoInicial, tarjeta.Saldo);
            Assert.AreEqual(0m, tarjeta.SaldoPendiente);
        }

        [Test]
        public void Test_AcreditarCarga_ConEspacioSuficiente_AcreditaTodo()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); // Saldo: 56000, Pendiente: 4000
            tarjeta.Descontar(10000); // Saldo: 46000, Pendiente: 4000

            //se llama automáticamente en Descontar, pero lo verificamos
            // El Descontar ya llamó a AcreditarCarga

            Assert.AreEqual(50000m, tarjeta.Saldo); // 46000 + 4000
            Assert.AreEqual(0m, tarjeta.SaldoPendiente);
        }

        [Test]
        public void Test_AcreditarCarga_ConEspacioInsuficiente_AcreditaParcial()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); // Saldo: 56000, Pendiente: 4000
            tarjeta.Descontar(1000); // Saldo: 55000, se acredita 1000, quedan 3000 pendientes

            //Descontar ya llamó a AcreditarCarga
            Assert.AreEqual(LIMITE_SALDO, tarjeta.Saldo);
            Assert.AreEqual(3000m, tarjeta.SaldoPendiente);
        }

        [Test]
        public void Test_AcreditarCarga_EspacioExacto_AcreditaTodoYCero()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); // Saldo: 56000, Pendiente: 4000
            tarjeta.Descontar(4000); // Saldo: 52000, espacio: 4000, pendiente: 4000

            Assert.AreEqual(LIMITE_SALDO, tarjeta.Saldo);
            Assert.AreEqual(0m, tarjeta.SaldoPendiente);
        }

        [Test]
        public void Test_Descontar_ConSaldoSuficiente_RetornaTrue()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);
            
            bool resultado = tarjeta.Descontar(2000);

            Assert.IsTrue(resultado);
            Assert.AreEqual(3000m, tarjeta.Saldo);
        }

        [Test]
        public void Test_Descontar_ConSaldoInsuficiente_UsaSaldoNegativo()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000);

            // Con saldo negativo, permite hasta -1200
            bool resultado = tarjeta.Descontar(3000);

            Assert.IsTrue(resultado, "Debe permitir con saldo negativo");
            Assert.AreEqual(-1000m, tarjeta.Saldo, "El saldo debe quedar en -1000");
        }

        [Test]
        public void Test_Descontar_ExactamenteTodoElSaldo_SaldoCero()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            bool resultado = tarjeta.Descontar(5000);

            Assert.IsTrue(resultado);
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void Test_Descontar_LlamaAcreditarCarga_Automaticamente()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); // Saldo: 56000, Pendiente: 4000

            tarjeta.Descontar(2000);

            //Verifica que se acreditó automáticamente
            Assert.AreEqual(LIMITE_SALDO, tarjeta.Saldo);
            Assert.AreEqual(2000m, tarjeta.SaldoPendiente);
        }

        [Test]
        public void Test_PagarPasaje_ConSaldoSuficiente_RetornaTrue()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            bool resultado = tarjeta.PagarPasaje();

            Assert.IsTrue(resultado);
            Assert.AreEqual(5000m - TARIFA_BASICA, tarjeta.Saldo);
        }

        [Test]
        public void Test_PagarPasaje_ConSaldoInsuficiente_UsaSaldoNegativo()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000);
            tarjeta.PagarPasaje(); // Queda con 420

            // Ahora permite viaje con saldo negativo
            bool resultado = tarjeta.PagarPasaje();

            Assert.IsTrue(resultado, "Debe permitir viaje con saldo negativo");
            Assert.AreEqual(-1160m, tarjeta.Saldo, "Saldo: 420 - 1580 = -1160");
        }

        [Test]
        public void Test_PagarPasaje_DescuentaTarifaBasica()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(10000);
            decimal saldoInicial = tarjeta.Saldo;

            tarjeta.PagarPasaje();

            Assert.AreEqual(saldoInicial - TARIFA_BASICA, tarjeta.Saldo);
        }

        [Test]
        public void Test_PagarPasaje_ConSaldoPendiente_AcreditaAutomaticamente()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); // Saldo: 56000, Pendiente: 4000

            bool resultado = tarjeta.PagarPasaje();

            Assert.IsTrue(resultado);
            Assert.AreEqual(LIMITE_SALDO, tarjeta.Saldo);
            Assert.AreEqual(2420m, tarjeta.SaldoPendiente);
        }

        [Test]
        public void Test_PagarPasaje_MultiplesVeces_DescuentaCorrectamente()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(10000);

            tarjeta.PagarPasaje(); // 10000 - 1580 = 8420
            tarjeta.PagarPasaje(); // 8420 - 1580 = 6840
            tarjeta.PagarPasaje(); // 6840 - 1580 = 5260

            Assert.AreEqual(5260m, tarjeta.Saldo);
        }

        [Test]
        public void Test_ObtenerTarifa_RetornaTarifaBasica()
        {
            Tarjeta tarjeta = new Tarjeta();

            decimal tarifa = tarjeta.ObtenerTarifa();

            Assert.AreEqual(TARIFA_BASICA, tarifa);
        }

        [Test]
        public void Test_ObtenerTipo_RetornaNormal()
        {
            Tarjeta tarjeta = new Tarjeta();

            string tipo = tarjeta.ObtenerTipo();

            Assert.AreEqual("Normal", tipo);
        }

        [Test]
        public void Test_MultiplesCargas_AcumulanSaldoPendiente()
        {
            Tarjeta tarjeta = new Tarjeta();
            
            tarjeta.Cargar(30000);  // Saldo: 30000
            tarjeta.Cargar(30000);  // Saldo: 56000, Pendiente: 4000
            tarjeta.Cargar(5000);   // Saldo: 56000, Pendiente: 9000
            tarjeta.Cargar(3000);   // Saldo: 56000, Pendiente: 12000

            Assert.AreEqual(LIMITE_SALDO, tarjeta.Saldo);
            Assert.AreEqual(12000m, tarjeta.SaldoPendiente);
        }

        [Test]
        public void Test_IntegracionCompleta_CargaViajesYAcreditacion()
        {
            Tarjeta tarjeta = new Tarjeta();

            //Cargar hasta superar límite
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); // Saldo: 56000, Pendiente: 4000
            Assert.AreEqual(LIMITE_SALDO, tarjeta.Saldo);
            Assert.AreEqual(4000m, tarjeta.SaldoPendiente);

            // Primer viaje
            tarjeta.PagarPasaje();
            Assert.AreEqual(LIMITE_SALDO, tarjeta.Saldo);
            Assert.AreEqual(2420m, tarjeta.SaldoPendiente);

            // Segundo viaje
            tarjeta.PagarPasaje();
            Assert.AreEqual(LIMITE_SALDO, tarjeta.Saldo);
            Assert.AreEqual(840m, tarjeta.SaldoPendiente);

            // Tercer viaje - agota el pendiente
            tarjeta.PagarPasaje();
            Assert.AreEqual(55260m, tarjeta.Saldo);
            Assert.AreEqual(0m, tarjeta.SaldoPendiente);

            // Cuarto viaje - sin pendiente
            tarjeta.PagarPasaje();
            Assert.AreEqual(53680m, tarjeta.Saldo);
            Assert.AreEqual(0m, tarjeta.SaldoPendiente);
        }

        [Test]
        public void Test_CargaMinimaYMaxima_FuncionanCorrectamente()
        {
            Tarjeta tarjeta1 = new Tarjeta();
            Tarjeta tarjeta2 = new Tarjeta();

            bool cargaMinima = tarjeta1.Cargar(2000);
            bool cargaMaxima = tarjeta2.Cargar(30000);

            Assert.IsTrue(cargaMinima);
            Assert.AreEqual(2000m, tarjeta1.Saldo);
            
            Assert.IsTrue(cargaMaxima);
            Assert.AreEqual(30000m, tarjeta2.Saldo);
        }

        [Test]
        public void Test_SaldoPendiente_SeAcreditaProgresivamente()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); // Pendiente: 4000

            tarjeta.Descontar(500);
            Assert.AreEqual(LIMITE_SALDO, tarjeta.Saldo);
            Assert.AreEqual(3500m, tarjeta.SaldoPendiente);

            tarjeta.Descontar(500);
            Assert.AreEqual(LIMITE_SALDO, tarjeta.Saldo);
            Assert.AreEqual(3000m, tarjeta.SaldoPendiente);

            tarjeta.Descontar(500);
            Assert.AreEqual(LIMITE_SALDO, tarjeta.Saldo);
            Assert.AreEqual(2500m, tarjeta.SaldoPendiente);
        }

        [Test]
        public void Test_SaldoNegativo_PermiteHasta1200()
        {
            Tarjeta tarjeta = new Tarjeta();
            
            bool resultado = tarjeta.Descontar(1200m);
            
            Assert.IsTrue(resultado, "Debe permitir saldo negativo hasta -1200");
            Assert.AreEqual(-1200m, tarjeta.Saldo);
        }

        [Test]
        public void Test_SaldoNegativo_NoPermiteSuperarLimite()
        {
            Tarjeta tarjeta = new Tarjeta();
            
            bool resultado = tarjeta.Descontar(1201m);
            
            Assert.IsFalse(resultado, "No debe permitir saldo menor a -1200");
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void Test_RecargaConSaldoNegativo_DescuentaDeuda()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Descontar(500m); // Saldo = -500
            
            tarjeta.Cargar(3000m);
            
            Assert.AreEqual(2500m, tarjeta.Saldo, "Debe descontar la deuda: -500 + 3000 = 2500");
        }

        #endregion
    }
}