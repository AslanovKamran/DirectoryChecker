﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDirectoryChechkerApp
{
	public class Person
	{
		public string Name { get; set; }
		public Person(string name)
		{
			Name = name;
		}
		public override string ToString() => $"{Name}";
	}
}