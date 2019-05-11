﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNTK;

namespace EasyCNTK.Layers
{
    /// <summary>
    /// Реализует слой Dropout
    /// </summary>
    public sealed class Dropout : Layer
    {
        private double _dropoutRate;
        private uint _seed;
        private string _name;

        /// <summary>
        ///  Применяет функцию дропаут к последнему добавленному слою
        /// </summary>
        /// <param name="input">Входной слой</param>
        /// <param name="dropoutRate">Доля отключаемых нейронов в слое</param>
        /// <param name="seed"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Function Build(Function input, double dropoutRate, uint seed = 0, string name = "Dropout")
        {
            return CNTKLib.Dropout(input, dropoutRate, seed, name);
        }
        public Dropout(double dropoutRate, uint seed = 0, string name = "Dropout")
        {
            _dropoutRate = dropoutRate;
            _seed = seed;
            _name = name;
        }

        public override Function Create(Function input, DeviceDescriptor device)
        {
            return CNTKLib.Dropout(input, _dropoutRate, _seed, _name);
        }
      
        public override string GetDescription()
        {
            return $"DO({_dropoutRate})";
        }
    }
}
