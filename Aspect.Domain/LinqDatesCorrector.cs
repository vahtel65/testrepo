using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aspect.Domain
{
    public partial class TechnReadiness
    {
        partial void OnLoaded()
        {
            if (this._him_date.HasValue) this._him_date = DateTime.SpecifyKind(this._him_date.Value, DateTimeKind.Utc);
            if (this._svar_date.HasValue) this._svar_date = DateTime.SpecifyKind(this._svar_date.Value, DateTimeKind.Utc);
            if (this._techn_date.HasValue) this._techn_date = DateTime.SpecifyKind(this._techn_date.Value, DateTimeKind.Utc);
        }

    }
}