﻿using System;
using System.Text;
using dotenv.net.DependencyInjection.Extensions;
using dotenv.net.Interfaces;
using dotenv.net.Utilities;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace dotenv.net.Test.DependencyInjection.Extensions
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void ShouldThrowWhenServicesAreNull()
        {
            Action action = () => ServiceCollectionExtensions.AddEnv(null, builder => { });
            action.Should()
                .ThrowExactly<ArgumentNullException>();
        }
        
        [Fact]
        public void ShouldThrowWhenSetupActionIsNull()
        {
            var services = new ServiceCollection();
            Action action = () => ServiceCollectionExtensions.AddEnv(services, null);
            action.Should()
                .ThrowExactly<ArgumentNullException>();
        }
        
        [Fact]
        public void ShouldWorkWhenSetupActionIsValid()
        {
            var services = new ServiceCollection();
            Action action = () => ServiceCollectionExtensions.AddEnv(services, options =>
            {
                options.AddEnvFile(".env");
                    options.AddEncoding(Encoding.UTF8);
                    options.AddThrowOnError(false);
                });
            action.Should()
                .NotThrow<Exception>();
        }
        
        [Fact]
        public void ShouldWorkWithAutomatedOptions()
        {
            var services = new ServiceCollection();
            Action action = () => ServiceCollectionExtensions.AddEnv(services);
            action.Should()
                .NotThrow<Exception>();
        }
        
        [Fact]
        public void ShouldThrowWhenServicesAreProvided()
        {
            Action action = () => ServiceCollectionExtensions.AddEnvReader(null);
            action.Should()
                .ThrowExactly<ArgumentNullException>();
        }
        
        [Fact]
        public void ShouldInjectEnvReaderWhenServicesAreProvided()
        {
            var services = new ServiceCollection();
            services.AddEnvReader();
            var provider = services.BuildServiceProvider();
            
            object service = null;
            Action action = () => service = (EnvReader) provider.GetService(typeof(IEnvReader));
            
            action.Should()
                .NotThrow<Exception>();
            service.Should()
                .NotBeNull();
            service.Should()
                .BeOfType<EnvReader>();
        }
    }
}