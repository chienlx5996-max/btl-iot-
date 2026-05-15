"""Database package for the smart home project.

This package exposes the main database helper and maintenance utilities.
"""

from .db_helper import DBHelper
from .db_setup import init_database


__all__ = ["DBHelper", "init_database", "migrate_database"]
