/*
    CompiledDomainModel Sitecore module
    Copyright (C) 2010-2011  Robin Hermanussen

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Pipelines;
using CompiledDomainModel.Utils;
using Sitecore.Diagnostics;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;

namespace CompiledDomainModel.Validation
{
    public class ValidateDomainModelOnStartup
    {
        public void Process(PipelineArgs args)
        {
            bool validationEnabled = false;
            bool throwExceptionOnFailed = false;
            try
            {
                Item settingsItem = ConfigurationUtil.SettingsItem;
                CheckboxField enableValidationField = FieldTypeManager.GetField(settingsItem.Fields["Enable validation on startup"]) as CheckboxField;
                Assert.IsNotNull(enableValidationField, "The field 'Enable validation on startup' could not be found");
                validationEnabled = enableValidationField.Checked;

                CheckboxField throwExceptionOnFailedField = FieldTypeManager.GetField(settingsItem.Fields["Throw exception on startup validation failed"]) as CheckboxField;
                Assert.IsNotNull(throwExceptionOnFailedField, "The field 'Throw exception on startup validation failed' could not be found");
                throwExceptionOnFailed = throwExceptionOnFailedField.Checked;
            }
            catch(Exception exc)
            {
                Log.Warn("Unable to get settings for Domain Model validation; skipping.", exc, this);
                validationEnabled = false;
            }
            if (validationEnabled)
            {
                List<string> validationMessages = new List<string>();
                ValidationUtil.ValidateDomainModel(validationMessages);
                if (validationMessages == null || validationMessages.Count() == 0)
                {
                    Log.Info("The domain model has been SUCCESSFULLY validated (the code is consistent with the sitecore database(s))", this);
                }
                else
                {
                    string errorMessage = string.Format("The domain model has has FAILED validation (the code is inconsistent with the sitecore database(s)). The error messages where: {0}", string.Join(", ", validationMessages.ToArray()));
                    Log.Error(errorMessage, this);
                    if (throwExceptionOnFailed)
                    {
                        throw new Exception(errorMessage);
                    }
                }
            }
        }

        
    }
}