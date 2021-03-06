﻿using System;
using System.IO;
using System.Text;
using dotenv.net.DependencyInjection.Infrastructure;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Test
{
    public class DotEnvTests
    {
        private const string WhitespacesEnvFileName = "values-with-whitespaces.env";
        private const string WhitespacesCopyEnvFileName = "values-with-whitespaces-too.env";
        private const string ValuesAndCommentsEnvFileName = "values-and-comments.env";
        private const string NonExistentEnvFileName = "non-existent.env";

        [Fact]
        public void ShouldThrowExceptionWhenFileNameEmptyOrNull()
        {
            Action action = () => DotEnv.Config(true, null);
            action.ShouldThrowExactly<ArgumentException>()
                .WithMessage($"The file path cannot be null, empty or whitespace.{Environment.NewLine}Parameter name: filePath");
        }

        [Fact]
        public void ThrowsExceptionWithNonExistentEnvFileWhenThrowErrorIsTrue()
        {
            Action action = () => DotEnv.Config(true, NonExistentEnvFileName);
            action.ShouldThrowExactly<FileNotFoundException>()
                .WithMessage($"An environment file with path \"{NonExistentEnvFileName}\" does not exist.");
        }

        [Fact]
        public void DoesNotThrowExceptionWithNonExistentEnvFileWhenThrowErrorIsFalse()
        {
            var dotEnvOptions = new DotEnvOptions
            {
                Encoding = Encoding.UTF8,
                ThrowOnError = false,
                EnvFile = NonExistentEnvFileName,
                TrimValues = true
            };
            Action action = () => DotEnv.Config(dotEnvOptions);
            action.ShouldNotThrow();
        }

        [Fact]
        public void AddsEnvironmentVariablesIfADefaultEnvFileExists()
        {
            Action action = () => DotEnv.Config();
            action.ShouldNotThrow();

            Environment.GetEnvironmentVariable("hello").Should().Be("world");
        }

        [Fact]
        public void AddsEnvironmentVariablesAndSetsValueAsNullIfNoneExists()
        {
            Action action = () => DotEnv.Config();
            action.ShouldNotThrow();

            Environment.GetEnvironmentVariable("strongestavenger").Should().Be(null);
        }

        [Fact]
        public void AllowsEnvFilePathToBeSpecified()
        {
            Action action = () => DotEnv.Config(true, ValuesAndCommentsEnvFileName);
            action.ShouldNotThrow();

            Environment.GetEnvironmentVariable("me").Should().Be("winner");
        }

        [Fact]
        public void ShouldReturnUntrimmedValuesWhenTrimIsFalse()
        {
            DotEnv.Config(true, WhitespacesEnvFileName, Encoding.UTF8, false);

            Environment.GetEnvironmentVariable("DB_CONNECTION").Should().Be("mysql  ");
            Environment.GetEnvironmentVariable("DB_HOST").Should().Be("127.0.0.1");
            Environment.GetEnvironmentVariable("DB_PORT").Should().Be("  3306");
            Environment.GetEnvironmentVariable("DB_DATABASE").Should().Be("laravel");
        }

        [Fact]
        public void ShouldReturnTrimmedValuesWhenTrimIsTrue()
        {
            DotEnv.Config(true, WhitespacesCopyEnvFileName, Encoding.UTF8, true);

            Environment.GetEnvironmentVariable("B_CONNECTION").Should().Be("mysql");
            Environment.GetEnvironmentVariable("B_HOST").Should().Be("127.0.0.1");
            Environment.GetEnvironmentVariable("B_PORT").Should().Be("3306");
            Environment.GetEnvironmentVariable("B_DATABASE").Should().Be("laravel");
        }
    }
}