using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ComputePower.Models;
using ComputePower.WPF.Models;

namespace ComputePower.WPF
{
    public static class ComputeMapper
    {
        private static bool _isInitialized;

        public static void Initialize()
        {
            if (!_isInitialized)
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Project, ProjectViewModel>();
                });
                _isInitialized = true;
            }
        }
    }
}
