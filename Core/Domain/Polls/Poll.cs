﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Localization;

namespace Core.Domain.Polls
{
    /// <summary>
    /// Represents a poll(民意调查)
    /// </summary>
    public partial class Poll : BaseEntity
    {
        private ICollection<PollAnswer> _pollAnswers;

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public int LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the system keyword
        /// </summary>
        public string SystemKeyword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity should be shown on home page
        /// </summary>
        public bool ShowOnHomePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the anonymous votes are allowed
        /// </summary>
        public bool AllowGuestsToVote { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the poll start date and time
        /// </summary>
        public DateTime? StartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the poll end date and time
        /// </summary>
        public DateTime? EndDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the news comments
        /// </summary>
        public virtual ICollection<PollAnswer> PollAnswers
        {
            get { return _pollAnswers ?? (_pollAnswers = new List<PollAnswer>()); }
            protected set { _pollAnswers = value; }
        }

        /// <summary>
        /// Gets or sets the language
        /// </summary>
        public virtual Language Language { get; set; }
    }
}
