﻿using Automatica.Core.Driver;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace P3.Driver.Blockchain.Ticker.Driver.Bitcoin
{
    internal class BlockchainValue
    {
        [JsonProperty("last")]
        public double Last { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }
    }


    internal class BitcoinValueNode : DriverBase
    {
        private readonly string _currency;
        private readonly bool _addSymbol;

        public BitcoinValueNode(IDriverContext driverContext, string currency, bool addSymbol=false) : base(driverContext)
        {
            _currency = currency;
            _addSymbol = addSymbol;
        }

        public void UpdateValue(JObject jObject)
        {
            if(jObject.ContainsKey(_currency))
            {
                var valueEntry = jObject[_currency].ToString();

                var blockChainValue = JsonConvert.DeserializeObject<BlockchainValue>(valueEntry);

                var value = $"{blockChainValue.Last}";

                if (_addSymbol)
                {
                    value += blockChainValue.Symbol;
                }

                DispatchValue(value);

                DriverContext.Logger.LogDebug($"Read value {blockChainValue.Last}{blockChainValue.Symbol}");
            }
        }

        public override IDriverNode CreateDriverNode(IDriverContext ctx)
        {
            return null;
        }
    }
}