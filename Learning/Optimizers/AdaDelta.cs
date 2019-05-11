﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNTK;

namespace EasyCNTK.Learning.Optimizers
{
    /// <summary>
    /// Оптимизатор AdaDelta. Улучшенная версия <seealso cref="AdaGrad"/>
    /// </summary>
    public class AdaDelta : Optimizer
    {
        private double _l1RegularizationWeight;
        private double _l2RegularizationWeight;
        private double _gradientClippingThresholdPerSample;        
        private int _minibatchSize;
        private double _rho;
        private double _epsilon;

        public override double LearningRate { get; }
        /// <summary>
        /// Инициализирует оптимизатор AdaDelta
        /// </summary>
        /// <param name="learningRate">Скорость обучения</param>
        /// <param name="minibatchSize">Размер минипакета, требуется CNTK чтобы масштабировать параметры оптимизатора для более эффективного обучения</param>
        /// <param name="epsilon">Константа для стабилизации(защита от деления на 0). Параметр "е" - в формуле правила обновления параметров: http://ruder.io/optimizing-gradient-descent/index.html#adadelta</param>
        /// <param name="rho">Экспоненциальный коэффициент сглаживания для каждого минипакета.</param>
        /// <param name="l1RegularizationWeight">Коэффициент L1 нормы, если 0 - регуляризация не применяется</param>
        /// <param name="l2RegularizationWeight">Коэффициент L2 нормы, если 0 - регуляризация не применяется</param>
        /// <param name="gradientClippingThresholdPerSample">Порог отсечения градиента на каждый пример обучения, используется преимущественно для борьбы с взрывным градиентом в глубоких реккурентных сетях.
        /// По умолчанию установлен в <seealso cref="double.PositiveInfinity"/> - отсечение не используется. Для использования установите необходимый порог.</param>
        public AdaDelta(double learningRate,
            int minibatchSize,
            double epsilon = 1e-8,
            double rho = 1,
            double l1RegularizationWeight = 0,
            double l2RegularizationWeight = 0,
            double gradientClippingThresholdPerSample = double.PositiveInfinity)
        {
            LearningRate = learningRate;
            _minibatchSize = minibatchSize;
            _l1RegularizationWeight = l1RegularizationWeight;
            _l2RegularizationWeight = l2RegularizationWeight;
            _gradientClippingThresholdPerSample = gradientClippingThresholdPerSample;
            _epsilon = epsilon;
            _rho = rho;
        }
        public override Learner GetOptimizer(IList<Parameter> learningParameters)
        {
            var learningOptions = new AdditionalLearningOptions()
            {
                l1RegularizationWeight = _l1RegularizationWeight,
                l2RegularizationWeight = _l2RegularizationWeight,
                gradientClippingWithTruncation = _gradientClippingThresholdPerSample != double.PositiveInfinity,
                gradientClippingThresholdPerSample = _gradientClippingThresholdPerSample
            };
            return CNTKLib.AdaDeltaLearner(new ParameterVector((ICollection)learningParameters), 
                new TrainingParameterScheduleDouble(LearningRate, (uint)_minibatchSize),
                _rho,
                _epsilon,
                learningOptions);
        }
    }
}
