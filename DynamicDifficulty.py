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

        self.slider_ranges = {
            "Positive Coin Power Up": (-5, 10),
            "Negative Coin Power Up": (-5, 10),
            "Final Power Up": (-5, 20),
            "Clash Power Up": (-5, 20),
            "Min Speed Adder": (-10, 10),
            "Max Speed Adder": (-10, 10),
            "Max HP Multiplier": (0.00, 5.00),
            "Defense Level": (-20, 20),
            "Offense Level": (-50, 50),
            "Damage Taken": (0.00, 2.00),
            "Damage Dealt": (0.00, 2.00),
            "Encounter Start Shield": (0, 500),
            "Combat Start Shield (Stacking)": (0, 500),
            "Combat Start Shield (Non-Stacking)": (0, 500),
            "Slash Resistance": (-2.0, 2.0),
            "Pierce Resistance": (-2.0, 2.0),
            "Blunt Resistance": (-2.0, 2.0),
            "Bonus Damage On Hit": (0, 200),
            "Bonus Flat Healing On Hit": (0, 200),
            "Round Start SP Healing": (-45, 45),
            "Clash Win SP Healing": (-45, 45),
            "Clash Lose SP Healing": (-45, 45)
        }

        self.keys = [
            "Positive Coin Power Up",
            "Negative Coin Power Up",
            "Final Power Up",
            "Clash Power Up",
            "Min Speed Adder",
            "Max Speed Adder",
            "Max HP Multiplier",
            "Defense Level",
            "Offense Level",
            "Damage Taken",
            "Damage Dealt",
            "Encounter Start Shield",
            "Combat Start Shield (Stacking)",
            "Combat Start Shield (Non-Stacking)",
            "Slash Resistance",
            "Pierce Resistance",
            "Blunt Resistance",
            "Bonus Damage On Hit",
            "Bonus Flat Healing On Hit",
            "Bonus Flat Healing On Combat Start",
            "Change Stagger On Self On Hit",
            "Change Stagger On Self When Hit",
            "Round Start SP Healing",
            "Clash Win SP Healing",
            "Clash Lose SP Healing"
        ]

        self.enemy_data = {}
        self.player_data = {}

        self.enemy_entries = {}
        self.player_entries = {}

        self.enemy_sliders = {}
        self.player_sliders = {}

        button_frame = tk.Frame(root)
        button_frame.pack(pady=10)

        self.choose_button = tk.Button(
            button_frame,
            text="Select Limbus Company Folder",
            command=self.load_json
        )
        self.choose_button.pack(side="left", padx=5)

        self.save_button = tk.Button(
            button_frame,
            text="Save Changes",
            command=self.save_json
        )
        self.save_button.pack(side="left", padx=5)


        self.canvas = tk.Canvas(root)
        self.scroll_y = tk.Scrollbar(root, orient="vertical", command=self.canvas.yview)
        self.frame = tk.Frame(self.canvas)

        self.frame.bind("<Configure>", lambda e: self.canvas.configure(scrollregion=self.canvas.bbox("all")))

        self.canvas.create_window((0, 0), window=self.frame, anchor="nw")
        self.canvas.configure(yscrollcommand=self.scroll_y.set)

        self.canvas.pack(side="left", fill="both", expand=True)
        self.scroll_y.pack(side="right", fill="y")

        self.canvas.bind_all("<MouseWheel>", self.handle_mousewheel)
        self.canvas.bind_all("<Button-4>", self.handle_mousewheel)
        self.canvas.bind_all("<Button-5>", self.handle_mousewheel) 

    def handle_enter(self, event):
        self.save_json()
    
    def handle_mousewheel(self, event):
        if event.delta:
            self.canvas.yview_scroll(int(-1 * (event.delta / 120)), "units")
        
        elif event.num == 4:
            self.canvas.yview_scroll(-1, "units")
        elif event.num == 5:
            self.canvas.yview_scroll(1, "units")


    def load_json(self):
        folder = filedialog.askdirectory(title="Select Limbus Company Folder")
        if not folder:
            return

        base = os.path.join(folder, "BepInEx", "plugins")
        self.enemy_path = os.path.join(base, "dynamicdifficultydata.json")
        self.player_path = os.path.join(base, "dynamicdifficultydataforplayers.json")

        os.makedirs(base, exist_ok=True)

        def load_or_create(path):
            if not os.path.exists(path):
                data = {k: 0 for k in self.keys}
                with open(path, "w", encoding="utf-8") as f:
                    json.dump(data, f, indent=4)
                return data
            with open(path, "r", encoding="utf-8") as f:
                return json.load(f)

        self.enemy_data = load_or_create(self.enemy_path)
        self.player_data = load_or_create(self.player_path)

        self.display_fields()


    def display_fields(self):
        for widget in self.frame.winfo_children():
            widget.destroy()

        self.enemy_entries.clear()
        self.player_entries.clear()
        self.enemy_sliders.clear()
        self.player_sliders.clear()

        row = 0

        def section_title(text):
            nonlocal row
            lbl = tk.Label(self.frame, text=text, font=("Arial", 12, "bold"))
            lbl.grid(row=row, column=0, columnspan=3, pady=(15, 5), sticky="w")
            row += 1

        def build_section(data, entries, sliders):
            nonlocal row
            for key in self.keys:
                value = data.get(key, 0)

                tk.Label(self.frame, text=key).grid(row=row, column=0, sticky="w", padx=5)

                entry = tk.Entry(self.frame, width=20)
                entry.grid(row=row, column=1, padx=5)
                entry.insert(0, str(value))

                min_val, max_val = self.slider_ranges.get(key, (-50, 200))
                slider = tk.Scale(
                    self.frame,
                    from_=min_val,
                    to=max_val,
                    orient="horizontal",
                    length=180,
                    resolution=0.1 if isinstance(value, float) else 1,
                )
                slider.grid(row=row, column=2, padx=5)
                slider.set(value)

                def slider_to_entry(val, entry=entry, is_float=isinstance(value, float)):
                    entry.delete(0, tk.END)
                    entry.insert(
                        0,
                        str(round(float(val), 2) if is_float else int(float(val)))
                    )

                def entry_to_slider(event, slider=slider):
                    try:
                        slider.set(float(entry.get()))
                    except ValueError:
                        pass

                slider.config(command=slider_to_entry)
                entry.bind("<KeyRelease>", entry_to_slider)


                entries[key] = entry
                sliders[key] = slider
                row += 1

        section_title("Enemy Modifiers")
        build_section(self.enemy_data, self.enemy_entries, self.enemy_sliders)

        section_title("Player Modifiers")
        build_section(self.player_data, self.player_entries, self.player_sliders)

    def save_json(self):
        def extract(entries, target):
            for k, e in entries.items():
                val = e.get()
                try:
                    target[k] = float(val) if "." in val else int(val)
                except ValueError:
                    target[k] = val

        extract(self.enemy_entries, self.enemy_data)
        extract(self.player_entries, self.player_data)

        with open(self.enemy_path, "w", encoding="utf-8") as f:
            json.dump(self.enemy_data, f, indent=4)

        with open(self.player_path, "w", encoding="utf-8") as f:
            json.dump(self.player_data, f, indent=4)

        messagebox.showinfo("Success", "Enemy and Player modifiers saved!")

if __name__ == "__main__":
    root = tk.Tk()
    app = JsonEditorApp(root)
    root.mainloop()
