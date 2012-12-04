using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
    public abstract class ChangesetInfoBase 
    {
        [DebuggerStepThrough]
        public ChangesetInfoBase()
        {
        }

        [DebuggerStepThrough]
        public ChangesetInfoBase(Int32 id, String username, DateTime timestamp, Int32 computationId)
        {
            this.Id = id;
            this.Username = username;
            this.Timestamp = timestamp;
            this.CalculationId = computationId;
        }

        /// <summary>
        /// ID column.
        /// ID of the changeset.
        /// </summary>
        public Int32 Id { get; set; }
        
        /// <summary>
        /// USERNAME column.
        /// User who made this changeset.
        /// </summary>
        public String Username { get; set; }

        /// <summary>
        /// TIMESTAMP column.
        /// Date and time when the changeset was made.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// ID of the computation this changeset belongs.
        /// </summary>
        public Int32 CalculationId { get; set; }

		public abstract void Accept(IChangesetInfoResolver resolver);
    }
}
