﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir_project
{
    public class TermDocumentMatrix
    {
        private Dictionary<String, Term> terms = new Dictionary<String, Term>();

        /// <summary>
        /// Returns a term that corresponds to a given value.
        /// A new term is created if such term doesn't exist.
        /// </summary>
        public Term getTerm(String value)
        {
            Term term = null;
            if (terms.TryGetValue(value, out term))
            {
                return term;
            }
            term = new Term(value);
            terms.Add(value, term);
            return term;
        }
    }
}