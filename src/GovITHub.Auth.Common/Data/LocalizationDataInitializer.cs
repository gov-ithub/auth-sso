using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Localization.SqlLocalizer.DbStringLocalizer;
using Microsoft.EntityFrameworkCore;

namespace GovITHub.Auth.Common.Data
{
    public class LocalizationDataInitializer
    {
        private LocalizationModelContext context;
        private IStringExtendedLocalizerFactory localizer;

        public LocalizationDataInitializer(LocalizationModelContext ctx, IStringExtendedLocalizerFactory localizer)
        {
            this.context = ctx;
            this.localizer = localizer;
        }

        public void InitializeData()
        {
            context.Database.Migrate();

            var items = readStream();
            if (!context.LocalizationRecords.Any())
            {
                
                localizer.AddNewLocalizationData(items, "New import");
            }
            else
            {
                // throws error Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException: Database operation expected to affect 1 row(s) but actually affected 0 row(s).
                //localizer.UpdatetLocalizationData(items, "Existing import");
            }
        }

        private List<LocalizationRecord> readStream()
        {
            var stream = File.OpenRead(Directory.GetCurrentDirectory() + "/localization/localizedData.csv");
            bool skipFirstLine = true;
            string csvDelimiter = ";";

            List<LocalizationRecord> list = new List<LocalizationRecord>();
            var reader = new StreamReader(stream);


            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(csvDelimiter.ToCharArray());
                if (skipFirstLine)
                {
                    skipFirstLine = false;
                }
                else
                {
                    var itemTypeInGeneric = list.GetType().GetTypeInfo().GenericTypeArguments[0];
                    var item = new LocalizationRecord();
                    var properties = item.GetType().GetProperties();
                    for (int i = 0; i < values.Length; i++)
                    {
                        properties[i].SetValue(item, Convert.ChangeType(values[i], properties[i].PropertyType), null);
                    }

                    list.Add(item);
                }
            }
            return list;
        }
    }
}
