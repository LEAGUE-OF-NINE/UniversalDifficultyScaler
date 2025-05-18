import tkinter as tk
from tkinter import filedialog, messagebox
import json
import os

class JsonEditorApp:
    def __init__(self, root):
        self.root = root
        self.root.title("Limbus Difficulty Editor")
        self.root.geometry("900x600")
        self.root.bind("<Return>", self.handle_enter)

        self.data = {}
        self.entries = {}
        self.sliders = {}

        self.choose_button = tk.Button(root, text="Select Limbus Company Folder", command=self.load_json)
        self.choose_button.pack(pady=10)

        self.canvas = tk.Canvas(root)
        self.scroll_y = tk.Scrollbar(root, orient="vertical", command=self.canvas.yview)
        self.frame = tk.Frame(self.canvas)

        self.frame.bind("<Configure>", lambda e: self.canvas.configure(scrollregion=self.canvas.bbox("all")))

        self.canvas.create_window((0, 0), window=self.frame, anchor="nw")
        self.canvas.configure(yscrollcommand=self.scroll_y.set)

        self.canvas.pack(side="left", fill="both", expand=True)
        self.scroll_y.pack(side="right", fill="y")

        self.save_button = tk.Button(root, text="Save Changes", command=self.save_json)
        self.save_button.pack(pady=10)

    def handle_enter(self, event):
        self.save_json()

    def load_json(self):
        folder = filedialog.askdirectory(title="Select Limbus Company Folder")
        if not folder:
            return

        json_path = os.path.join(folder, "BepInEx", "plugins", "data.json")

        if not os.path.exists(json_path):
            os.makedirs(os.path.dirname(json_path), exist_ok=True)

            default_keys = [
                "Negative Coin Power Up",
                "Final Power Up",
                "Clash Power Up",
                "Min Speed Adder",
                "Max Speed Adder",
                "Max HP Multiplier",
                "Defense Level",
                "Offense Level",
                "Damage Taken",
                "Damage Dealt"
            ]
            self.data = {key: 0 for key in default_keys}

            with open(json_path, "w", encoding="utf-8") as file:
                json.dump(self.data, file, indent=4)

            # messagebox.showinfo("Info", f"No data.json found. A new one was created at:\n{json_path}")
        else:
            with open(json_path, "r", encoding="utf-8") as file:
                try:
                    self.data = json.load(file)
                except json.JSONDecodeError as e:
                    messagebox.showerror("Error", f"Invalid JSON:\n{e}")
                    return

        self.json_path = json_path
        self.display_fields()


    def display_fields(self):
        for widget in self.frame.winfo_children():
            widget.destroy()

        self.entries.clear()
        self.sliders.clear()

        for i, (key, value) in enumerate(self.data.items()):
            tk.Label(self.frame, text=key).grid(row=i, column=0, sticky='w', padx=5, pady=5)

            entry = tk.Entry(self.frame, width=40)
            entry.grid(row=i, column=1, padx=5, pady=5)

            if isinstance(value, (int, float)):
                # Set default slider range: 0–200, can be adjusted
                slider = tk.Scale(
                    self.frame, from_=0, to=200,
                    orient="horizontal", length=150,
                    resolution=0.1 if isinstance(value, float) else 1,
                )
                slider.grid(row=i, column=2, padx=5, pady=5)

                entry.insert(0, str(value))
                slider.set(value)

                # Link slider and entry
                def make_sync_functions(k, is_float):
                    def slider_to_entry(val):
                        self.entries[k].delete(0, tk.END)
                        self.entries[k].insert(0, str(round(float(val), 2) if is_float else int(float(val))))
                    def entry_to_slider(event):
                        try:
                            v = float(self.entries[k].get())
                            self.sliders[k].set(v)
                        except ValueError:
                            pass
                    return slider_to_entry, entry_to_slider

                s2e, e2s = make_sync_functions(key, isinstance(value, float))
                slider.config(command=s2e)
                entry.bind("<KeyRelease>", e2s)

                self.sliders[key] = slider
            else:
                entry.insert(0, str(value))

            self.entries[key] = entry

    def save_json(self):
        for key, entry in self.entries.items():
            val = entry.get()
            try:
                if '.' in val:
                    self.data[key] = float(val)
                elif val.isdigit() or (val.startswith('-') and val[1:].isdigit()):
                    self.data[key] = int(val)
                else:
                    self.data[key] = val
            except ValueError:
                self.data[key] = val

        with open(self.json_path, "w", encoding="utf-8") as file:
            json.dump(self.data, file, indent=4)

        messagebox.showinfo("Success", "Changes saved!")

if __name__ == "__main__":
    root = tk.Tk()
    app = JsonEditorApp(root)
    root.mainloop()
