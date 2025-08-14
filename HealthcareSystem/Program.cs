using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareSystem
{
    // Generic Repository
    public class Repository<T>
    {
        private List<T> items = new List<T>();

        public void Add(T item)
        {
            items.Add(item);
        }

        public List<T> GetAll()
        {
            return items;
        }

        public T? GetById(Func<T, bool> predicate)
        {
            return items.FirstOrDefault(predicate);
        }

        public bool Remove(Func<T, bool> predicate)
        {
            var item = items.FirstOrDefault(predicate);
            if (item != null)
            {
                items.Remove(item);
                return true;
            }
            return false;
        }
    }

    // Patient Class
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }
    }

    // Prescription Class
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }
    }

    // Main Application
    public class HealthSystemApp
    {
        private Repository<Patient> _patientRepo;
        private Repository<Prescription> _prescriptionRepo;
        private Dictionary<int, List<Prescription>> _prescriptionMap;

        public HealthSystemApp()
        {
            _patientRepo = new Repository<Patient>();
            _prescriptionRepo = new Repository<Prescription>();
            _prescriptionMap = new Dictionary<int, List<Prescription>>();
        }

        public void SeedData()
        {
            // Add patients
            _patientRepo.Add(new Patient(1, "John Smith", 45, "Male"));
            _patientRepo.Add(new Patient(2, "Sarah Johnson", 32, "Female"));
            _patientRepo.Add(new Patient(3, "Michael Brown", 67, "Male"));

            // Add prescriptions
            _prescriptionRepo.Add(new Prescription(101, 1, "Lisinopril 10mg", DateTime.Now.AddDays(-30)));
            _prescriptionRepo.Add(new Prescription(102, 1, "Metformin 500mg", DateTime.Now.AddDays(-15)));
            _prescriptionRepo.Add(new Prescription(103, 2, "Ibuprofen 400mg", DateTime.Now.AddDays(-7)));
            _prescriptionRepo.Add(new Prescription(104, 2, "Vitamin D3", DateTime.Now.AddDays(-3)));
            _prescriptionRepo.Add(new Prescription(105, 3, "Atorvastatin 20mg", DateTime.Now.AddDays(-20)));
        }

        public void BuildPrescriptionMap()
        {
            foreach (var prescription in _prescriptionRepo.GetAll())
            {
                if (!_prescriptionMap.ContainsKey(prescription.PatientId))
                {
                    _prescriptionMap[prescription.PatientId] = new List<Prescription>();
                }
                _prescriptionMap[prescription.PatientId].Add(prescription);
            }
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("All Patients:");
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine($"ID: {patient.Id}, Name: {patient.Name}, Age: {patient.Age}, Gender: {patient.Gender}");
            }
        }

        public void PrintPrescriptionsForPatient(int patientId)
        {
            var patient = _patientRepo.GetById(p => p.Id == patientId);
            if (patient != null)
            {
                Console.WriteLine($"\nPrescriptions for {patient.Name}:");
                var prescriptions = GetPrescriptionsByPatientId(patientId);
                foreach (var prescription in prescriptions)
                {
                    Console.WriteLine($"- {prescription.MedicationName} (Issued: {prescription.DateIssued:yyyy-MM-dd})");
                }
            }
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            if (_prescriptionMap.ContainsKey(patientId))
            {
                return _prescriptionMap[patientId];
            }
            return new List<Prescription>();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var healthSystem = new HealthSystemApp();
            
            healthSystem.SeedData();
            healthSystem.BuildPrescriptionMap();
            healthSystem.PrintAllPatients();
            healthSystem.PrintPrescriptionsForPatient(1);
        }
    }
}