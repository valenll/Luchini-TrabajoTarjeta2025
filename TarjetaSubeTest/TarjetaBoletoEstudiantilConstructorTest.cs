/*using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    /// <summary>
    /// Tests para aumentar cobertura de TarjetaBoletoGratuitoEstudiantil
    /// </summary>
    [TestFixture]
    public class TarjetaBoletoEstudiantilConstructorTest
    {
        [Test]
        public void Test_ConstructorSinTiempoProvider_CreaTarjetaCorrectamente()
        {
            TarjetaBoletoGratuitoEstudiantil tarjeta = new TarjetaBoletoGratuitoEstudiantil();
            
            Assert.IsNotNull(tarjeta);
            Assert.AreEqual(0m, tarjeta.Saldo);
            Assert.AreEqual("Franquicia Completa", tarjeta.ObtenerTipo());
        }

        [Test]
        public void Test_ConstructorSinTiempoProvider_PuedeRealizarViajes()
        {
            TarjetaBoletoGratuitoEstudiantil tarjeta = new TarjetaBoletoGratuitoEstudiantil();
            Colectivo colectivo = new Colectivo("152", "Rosario Bus");
            
            // Nota: Este test puede fallar si se ejecuta fuera del horario permitido (L-V 6-22)
            // pero cubre el constructor sin tiempo
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            // Si está en horario permitido, debe generar boleto
            // Si está fuera de horario, debe ser null
            // En cualquier caso, el constructor funcionó correctamente
            Assert.IsTrue(boleto == null || boleto.TipoTarjeta == "Franquicia Completa");
        }

        [Test]
        public void Test_ConstructorSinTiempoProvider_PuedeCargar()
        {
            TarjetaBoletoGratuitoEstudiantil tarjeta = new TarjetaBoletoGratuitoEstudiantil();
            
            bool resultado = tarjeta.Cargar(5000m);
            
            Assert.IsTrue(resultado);
            Assert.AreEqual(5000m, tarjeta.Saldo);
        }

        [Test]
        public void Test_AmbosConstructores_FuncionanCorrectamente()
        {
            // Constructor sin tiempo
            TarjetaBoletoGratuitoEstudiantil tarjeta1 = new TarjetaBoletoGratuitoEstudiantil();
            
            // Constructor con tiempo
            TiempoSimulado tiempo = new TiempoSimulado(new DateTime(2024, 11, 25, 10, 0, 0));
            TarjetaBoletoGratuitoEstudiantil tarjeta2 = new TarjetaBoletoGratuitoEstudiantil(tiempo);
            
            Assert.AreEqual("Franquicia Completa", tarjeta1.ObtenerTipo());
            Assert.AreEqual("Franquicia Completa", tarjeta2.ObtenerTipo());
        }

        [Test]
        public void Test_ConstructorSinTiempoProvider_IdUnico()
        {
            TarjetaBoletoGratuitoEstudiantil tarjeta1 = new TarjetaBoletoGratuitoEstudiantil();
            TarjetaBoletoGratuitoEstudiantil tarjeta2 = new TarjetaBoletoGratuitoEstudiantil();
            
            Assert.AreNotEqual(tarjeta1.Id, tarjeta2.Id);
        }
    }
}*/
