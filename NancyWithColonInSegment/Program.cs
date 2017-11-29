using System;
using System.Linq;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Hosting.Self;
using Nancy.TinyIoc;

namespace NancyWithColonInSegment {
    class Program {
        static void Main(string[] args) {
            var url = "http://localhost:63579";
            var config = new HostConfiguration {UrlReservations = {CreateAutomatically = true}};
            using (var host = new NancyHost(config, new Uri(url))) {
                host.Start();
                Console.WriteLine($"Running on {url}");
                Console.ReadLine();
            }
        }
    }

    public class BootStrapper : DefaultNancyBootstrapper {
        protected override void ConfigureConventions(NancyConventions nancyConventions) {
            nancyConventions.StaticContentsConventions.Clear();
            nancyConventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("/", "/gui")
            );

            base.ConfigureConventions(nancyConventions);
        }
    }

    public class MainModule : NancyModule {

        private readonly Customer[] _customers = {
            new Customer("SantClauseInc:23", "Bernard the Elf"),
            new Customer("SantClauseInc:24", "Judy the Elf"),
        };

        public MainModule() : base("/api/v1/customer") {
            //customer_id contains colon
            Get["/{id}"] = parameters => GetCustomerById((string)parameters.id);
            Get["/list"] = _ => Response.AsJson(_customers);
        }

        private dynamic GetCustomerById(string id) {
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            return customer != null ? Response.AsJson(customer) : "404 - CUSTOM NOT FOUND";
        }
    }

    internal class Customer {
        public Customer(string id, string name) {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }
    }

    public class IndexModule : NancyModule {
        public IndexModule() {
            Get[@"/(.*)"] = _ => Response.AsFile("gui/index.html", "text/html");
            Get[@"/"] = _ => Response.AsFile("gui/index.html", "text/html");
        }
    }
}
