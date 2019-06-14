﻿using CNTK;
using EasyCNTK.Learning.Optimizers;
using EasyCNTK.LossFunctions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EasyCNTK.Learning.Reinforcement
{
    public abstract class Agent<T> : IDisposable where T : IConvertible
    {
        protected Environment Environment { get; set; }
        protected Sequential<T> Model { get; set; }
        protected DeviceDescriptor Device { get; set; }
        protected T[] Multiply(T[] vector, T factor)
        {
            var type = typeof(T);
            T[] result = new T[vector.Length];
            for (int i = 0; i < result.Length; i++)
            {
                double v = vector[i].ToDouble(CultureInfo.InvariantCulture);
                double f = factor.ToDouble(CultureInfo.InvariantCulture);
                result[i] = (T)Convert.ChangeType(v * f, type);
            }
            return result;
        }
        protected T CalculateDiscountedReward(T[] rewards, double gamma)
        {
            var type = typeof(T);
            double totalReward = rewards[0].ToDouble(CultureInfo.InvariantCulture);
            for (int i = 1; i < rewards.Length; i++)
            {
                totalReward += rewards[i].ToDouble(CultureInfo.InvariantCulture) * Math.Pow(gamma, i);
            }
            return (T)Convert.ChangeType(totalReward, type);
        }
        public Func<Loss> GetLoss { get; set; } = () => new SquaredError();
        public Func<Loss> GetEvalLoss { get; set; } = () => new SquaredError();
        public Func<int, Optimizer> GetOptimizer { get; set; } = (minibatchSize) => new Adam(0.05, 0.9, minibatchSize);

        public Agent(Environment environment, Sequential<T> model, DeviceDescriptor device)
        {
            Environment = environment;
            Model = model;
            Device = device;
        }

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Environment.Dispose();
                    Model.Dispose();
                    Device.Dispose();
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.
                Environment = null;
                Model = null;
                Device = null;

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        ~Agent()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(false);
        }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
