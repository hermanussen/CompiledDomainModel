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
using System.Web.Caching;
using Sitecore.Diagnostics;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;
using Sitecore.Pipelines;

namespace CompiledDomainModel.Validation
{
    public static class ValidationUtil
    {
        public static void ValidateDomainModel(List<string> errors)
        {
            ValidateDomainModel(errors, null);
        }

        public static void ValidateDomainModel(List<string> errors, List<string> warnings)
        {
            Assert.IsNotNull(errors, "You need to provide an 'errors' list for validation results");
            if (warnings == null)
            {
                warnings = new List<string>();
            }

            ValidateDomainModelPipelineArgs pipelineArgs = new ValidateDomainModelPipelineArgs(errors, warnings);
            CorePipeline.Run("ValidateDomainModel", pipelineArgs);
        }

    }
}