﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Diagnostics;

namespace Microsoft.MixedReality.Toolkit.SDK.DiagnosticsSystem
{
    internal class MemoryUseTracker
    {
        private Process currentProcess = Process.GetCurrentProcess();
        private readonly MemoryReading[] memoryReadings;
        private int index = 0;

        private MemoryReading sumReading = new MemoryReading();

        public MemoryUseTracker()
        {
            this.memoryReadings = new MemoryReading[10];
            for (int i = 0; i < this.memoryReadings.Length; i++)
            {
                this.memoryReadings[i] = new MemoryReading();
            }
        }


        public MemoryReading GetReading()
        {
            var reading = memoryReadings[index];

            reading.VirtualMemoryInBytes = currentProcess.VirtualMemorySize64;
            reading.WorkingSetMemoryInBytes = currentProcess.WorkingSet64;
            reading.GCMemoryInBytes = GC.GetTotalMemory(false);

            memoryReadings[index] = reading;
            index = (index + 1) % memoryReadings.Length;

            return memoryReadings.Aggregate(sumReading.Reset(), (a, b) => a + b) / memoryReadings.Length;
        }

        public struct MemoryReading
        {
            public long VirtualMemoryInBytes { get; set; }
            public long WorkingSetMemoryInBytes { get; set; }
            public long GCMemoryInBytes { get; set; }

            public MemoryReading Reset()
            {
                this.VirtualMemoryInBytes = 0;
                this.WorkingSetMemoryInBytes = 0;
                this.GCMemoryInBytes = 0;

                return this;
            }

            public static MemoryReading operator +(MemoryReading a, MemoryReading b)
            {
                a.VirtualMemoryInBytes += b.VirtualMemoryInBytes;
                a.WorkingSetMemoryInBytes += b.WorkingSetMemoryInBytes;
                a.GCMemoryInBytes += b.GCMemoryInBytes;

                return a;
            }

            public static MemoryReading operator /(MemoryReading a, int b)
            {
                a.VirtualMemoryInBytes /= b;
                a.WorkingSetMemoryInBytes /= b;
                a.GCMemoryInBytes /= b;

                return a;
            }
        }
    }
}
